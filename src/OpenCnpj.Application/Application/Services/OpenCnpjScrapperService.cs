using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;
using OpenCnpj.Application.Government.Clients.Interfaces;
using Serilog;
using System.Threading.Channels;

namespace OpenCnpj.Application.Application.Services;
public class OpenCnpjScrapperService(IBatchService batchService, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IMongoCollectionsService mongoCollectionsService, IBatchFileRepository batchFileRepository, ILogger logger) : IOpenCnpjScrapperService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjScrapperService>();

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

    //TODO: Aqui eu preciso pegar todos os batch files do batch passado por parâmetro e ver se eles existem primeiro, se existirem e tiverem com erro tem que retentar.
    private async Task<Result> ExecuteIncrementalPipeline(Batch batch, CancellationToken cancellationToken)
    {
        var batchFiles = await batchFileRepository.GetUnfinishedBatchFileOperationsByBatchID(batch.ID, cancellationToken);
        if (!batchFiles.Any())
            return await ExecuteAllIncrementalPipelines(batch, cancellationToken);

        // TODO: Pegar a menor operação que deu errado na lista acima e chamar apenas as pipelines que precisa pra fazer aquela dar certo. Criar novos métodos pra serem exatamente aqueles necessários de serem executados, por exemplo, se não precisa baixar tudo de novo, passa a URL por parametro e tenta fazer o download de novo, mesma coisa pra extrair e processar no strategy.

        return Result.Success();
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
