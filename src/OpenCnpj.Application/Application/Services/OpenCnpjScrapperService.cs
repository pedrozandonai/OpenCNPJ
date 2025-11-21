using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Government.Clients.Interfaces;
using Serilog;
using System.Threading.Channels;

namespace OpenCnpj.Application.Application.Services;
public class OpenCnpjScrapperService(IBatchFileService batchService, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IMongoCollectionsService mongoCollectionsService, ILogger logger) : IOpenCnpjScrapperService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjScrapperService>();

    public async Task<Result> ExecuteAsync(Batch batch, CancellationToken cancellationToken)
    {
        var pipelineResult = await ExecuteIncrementalPipeline(batch, cancellationToken);
        if (pipelineResult.IsFailure)
            return pipelineResult;

        bool finished = false;
        while (!finished)
        {
            var nextBatchOperationResult = batch.GetBatchNextOperation();
            if (nextBatchOperationResult.IsFailure)
            {
                var setOperationFailureResult = batch.SetOperationFailure(nextBatchOperationResult.Error);
                if (setOperationFailureResult.IsFailure)
                    return setOperationFailureResult;

                var setFinishedBatchResult = await batchService.UpdateBatch(batch, () => { return setOperationFailureResult; }, cancellationToken);
                if (setFinishedBatchResult.IsFailure)
                    return setFinishedBatchResult;

                return Result.Failure(nextBatchOperationResult.Error);
            }

            switch (nextBatchOperationResult.Value)
            {
                case EBatchOperation.PendingGovernmentBatch:
                    if (!batch.RetryDate.HasValue)
                        return Result.Failure("The batch is missing the retry date.");

                    await Task.Delay(DateTime.Now - batch.RetryDate.Value, cancellationToken);
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
        // Canais para comunicação entre etapas do pipeline
        var downloadedFilesChannel = Channel.CreateUnbounded<BatchFile>(new UnboundedChannelOptions
        {
            SingleWriter = false, // Múltiplos downloads simultâneos
            SingleReader = true   // Apenas o extractor lê
        });

        var extractedFilesChannel = Channel.CreateUnbounded<BatchFile>(new UnboundedChannelOptions
        {
            SingleWriter = true,  // Apenas o extractor escreve
            SingleReader = false  // Múltiplos processadores CSV podem ler
        });

        // Inicializa as tarefas do pipeline
        var downloadTask = Task.Run(async () =>
        {
            try
            {
                return await governmentHttpClient.DownloadCurrentBatch(batch, downloadedFilesChannel, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in download task");
                downloadedFilesChannel.Writer.Complete(ex);
                return Result.Failure($"Download error: {ex.Message}");
            }
        }, cancellationToken);

        var extractionTask = Task.Run(async () =>
        {
            try
            {
                return await fileExtractionService.ExtractFilesIncremental(batch, downloadedFilesChannel, extractedFilesChannel, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in extraction task");
                extractedFilesChannel.Writer.Complete(ex);
                return Result.Failure($"Extraction error: {ex.Message}");
            }
        }, cancellationToken);

        var processingTask = Task.Run(async () =>
        {
            try
            {
                return await csvProcessingService.ProcessCsvFilesIncremental(batch, extractedFilesChannel, cancellationToken);
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
            _logger.Information("Incremental pipeline completed successfully");
            return Result.Success();
        }

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));
        _logger.Error("Incremental pipeline failed: {0}", errors);

        return Result.Failure(errors);
    }
}
