using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Models.Dtos;
using OpenCnpj.Application.Batches.BatchFiles.Models.Enums;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;
using OpenCnpj.Application.Batches.Models.Enums;
using OpenCnpj.Application.Government.Clients.Interfaces;
using Serilog;
using System.Threading.Channels;

namespace OpenCnpj.Application.Application.Services;
public class OpenCnpjScrapperService(IBatchService batchService, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IMongoCollectionsService mongoCollectionsService, IBatchFileRepository batchFileRepository, ILogger logger) : IOpenCnpjScrapperService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjScrapperService>();

    private readonly Channel<BatchFile> _downloadedFilesChannel = Channel.CreateUnbounded<BatchFile>(new UnboundedChannelOptions
    {
        SingleWriter = false,
        SingleReader = true
    });

    private readonly Channel<BatchFile> _extractedFilesChannel = Channel.CreateUnbounded<BatchFile>(new UnboundedChannelOptions
    {
        SingleWriter = true,
        SingleReader = false
    });

    public async Task<Result> ExecuteAsync(Batch batch, CancellationToken cancellationToken)
    {
        bool finished = batch.Operation == EBatchOperation.Finished;
        while (!finished)
        {
            var nextBatchOperationResult = batch.GetBatchNextOperation();
            if (nextBatchOperationResult.IsFailure)
            {
                var setFinishedBatchResult = await batchService.UpdateBatch(batch, () => batch.SetOperationFailure(nextBatchOperationResult.Error), cancellationToken);
                if (setFinishedBatchResult.IsFailure)
                    return setFinishedBatchResult;

                return Result.Failure(nextBatchOperationResult.Error);
            }

            switch (nextBatchOperationResult.Value)
            {
                case EBatchOperation.PendingGovernmentBatch:
                    return Result.Success();
                    
                case EBatchOperation.StartGovernmentPipeline:
                    var pipelineResult = await ExecuteIncrementalPipeline(batch, cancellationToken);
                    if (pipelineResult.IsFailure)
                        return pipelineResult;
                    break;

                case EBatchOperation.RenamingMongoCollections:
                    var renameTemporaryCollectionsResult = await mongoCollectionsService.RenameTemporaryCollections(batch, cancellationToken);
                    if (renameTemporaryCollectionsResult.IsFailure)
                        return renameTemporaryCollectionsResult;
                    break;

                case EBatchOperation.Finished:
                    var setFinishedBatchResult = await batchService.UpdateBatch(batch, batch.SetFinishedBatch, cancellationToken);
                    if (setFinishedBatchResult.IsFailure)
                        return setFinishedBatchResult;

                    finished = true;
                    break;
            }
        }

        _logger.Information("Batch {0} completed successfully", batch.ID);
        return Result.Success();
    }

    private async Task<Result> ExecuteIncrementalPipeline(Batch batch, CancellationToken cancellationToken)
    {
        var batchFiles = await batchFileRepository.GetUnfinishedBatchFileOperationsByBatchID(batch.ID, cancellationToken);
        if (!batchFiles.Any())
            return await ExecuteAllIncrementalPipelines(batch, cancellationToken);

        _logger.Information("Found {Count} unfinished batch files for batch {BatchId}", batchFiles.Count(), batch.ID);

        // Identifica a menor operação que precisa ser retentada
        var minOperation = batchFiles.Min(bf => bf.FileOperation);

        _logger.Information("Minimum failed operation is {Operation} for batch {BatchId}. Will retry from this point forward.", minOperation, batch.ID);

        var failedBatchPartialDownloadFiles = batchFiles.Where(b =>
        b.FileOperation == EBatchFileOperation.Downloading &&
        b.Type == EBatchFileType.PartialDownloadedFile &&
        b.OperationStatus == EOperationStatus.Failure);

        var failedExtractedBatchFiles = batchFiles.Where(b =>
        b.FileOperation == EBatchFileOperation.Downloading &&
        b.Type == EBatchFileType.DownloadedFile &&
        b.OperationStatus == EOperationStatus.Failure);

        var failedProcessedBatchFiles = batchFiles.Where(b =>
        b.FileOperation == EBatchFileOperation.Processing &&
        b.Type == EBatchFileType.ExtractedFile &&
        b.OperationStatus == EOperationStatus.Failure);

        List<Task<Result>> tasksToExecute = [];

        if (failedBatchPartialDownloadFiles.Any())
            tasksToExecute.Add(Task.Run(async () =>
            {
                try
                {
                    return await governmentHttpClient.DownloadBatchFiles(failedBatchPartialDownloadFiles, _downloadedFilesChannel, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in download task");
                    _downloadedFilesChannel.Writer.Complete(ex);
                    return Result.Failure($"Download error: {ex.Message}");
                }
            }, cancellationToken));

        if (failedExtractedBatchFiles.Any())
        {
            foreach (var failedExtractedBatchFille in failedExtractedBatchFiles)
                await _downloadedFilesChannel.Writer.WriteAsync(failedExtractedBatchFille, cancellationToken: cancellationToken);

            tasksToExecute.Add(Task.Run(async () =>
            {
                try
                {
                    return await fileExtractionService.ExtractFilesIncremental(batch, _downloadedFilesChannel, _extractedFilesChannel, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in extraction task");
                    _extractedFilesChannel.Writer.Complete(ex);
                    return Result.Failure($"Extraction error: {ex.Message}");
                }
            }, cancellationToken));
        }

        if (failedProcessedBatchFiles.Any())
        {
            foreach (var failedProcessedBatchFille in failedProcessedBatchFiles)
                await _extractedFilesChannel.Writer.WriteAsync(failedProcessedBatchFille, cancellationToken: cancellationToken);

            tasksToExecute.Add(Task.Run(async () =>
            {
                try
                {
                    return await csvProcessingService.ProcessCsvFilesIncremental(batch, _extractedFilesChannel, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error in CSV processing task");
                    return Result.Failure($"CSV processing error: {ex.Message}");
                }
            }, cancellationToken));
        }

        var results = await Task.WhenAll(tasksToExecute);

        // Verifica se todas as etapas foram bem-sucedidas
        if (results.All(r => r.IsSuccess))
        {
            var setFinishedBatchResult = await batchService.UpdateBatch(batch, batch.SetGovernmentPipelineFinished, cancellationToken);
            if (setFinishedBatchResult.IsFailure)
                return setFinishedBatchResult;

            _logger.Information("Incremental pipeline completed successfully");

            return Result.Success();
        }

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));
        _logger.Error("Incremental pipeline failed: {0}", errors);

        var setBatchOperationFailureResult = await batchService.UpdateBatch(batch, () => batch.SetOperationFailure(errors), cancellationToken);
        if (setBatchOperationFailureResult.IsFailure)
            return setBatchOperationFailureResult;

        return Result.Failure(errors);
    }

    private async Task<Result> ExecuteAllIncrementalPipelines(Batch batch, CancellationToken cancellationToken)
    {
        var setBatchToGovernmentPipelineResult = await batchService.UpdateBatch(batch, batch.StartStartGovernmentPipeline, cancellationToken);
        if (setBatchToGovernmentPipelineResult.IsFailure)
        {
            _logger
                .ForContext("batch", batch, true)
                .Error("Unable to start government pipeline operation. Error: {0}", setBatchToGovernmentPipelineResult.Error);

            return Result.Failure(setBatchToGovernmentPipelineResult.Error);
        }

        // Inicializa as tarefas do pipeline
        var downloadTask = Task.Run(async () =>
        {
            try
            {
                return await governmentHttpClient.DownloadCurrentBatch(batch, _downloadedFilesChannel, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in download task");
                _downloadedFilesChannel.Writer.Complete(ex);
                return Result.Failure($"Download error: {ex.Message}");
            }
        }, cancellationToken);

        var extractionTask = Task.Run(async () =>
        {
            try
            {
                return await fileExtractionService.ExtractFilesIncremental(batch, _downloadedFilesChannel, _extractedFilesChannel, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in extraction task");
                _extractedFilesChannel.Writer.Complete(ex);
                return Result.Failure($"Extraction error: {ex.Message}");
            }
        }, cancellationToken);

        var processingTask = Task.Run(async () =>
        {
            try
            {
                return await csvProcessingService.ProcessCsvFilesIncremental(batch, _extractedFilesChannel, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in CSV processing task");
                return Result.Failure($"CSV processing error: {ex.Message}");
            }
        }, cancellationToken);

        // Aguarda todas as etapas do pipeline
        var results = await Task.WhenAll(downloadTask, extractionTask, processingTask);

        // Verifica se todas as etapas foram bem-sucedidas
        if (results.All(r => r.IsSuccess))
        {
            var setFinishedBatchResult = await batchService.UpdateBatch(batch, batch.SetGovernmentPipelineFinished, cancellationToken);
            if (setFinishedBatchResult.IsFailure)
                return setFinishedBatchResult;

            _logger.Information("Incremental pipeline completed successfully");

            return Result.Success();
        }

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));
        _logger.Error("Incremental pipeline failed: {0}", errors);

        var setBatchOperationFailureResult = await batchService.UpdateBatch(batch, () => batch.SetOperationFailure(errors), cancellationToken);
        if (setBatchOperationFailureResult.IsFailure)
            return setBatchOperationFailureResult;

        return Result.Failure(errors);
    }
}
