using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Domain;
using OpenCnpj.Application.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Services;
using OpenCnpj.Application.Government.Clients.Interfaces;
using Serilog;

namespace OpenCnpj.Application.Application.Services;
public class OpenCnpjScrapperService(IBatchService batchService, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IMongoCollectionsService mongoCollectionsService, ILogger logger) : IOpenCnpjScrapperService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjScrapperService>();

    public async Task<Result> ExecuteAsync(Batch batch, CancellationToken cancellationToken)
    {
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
                case EBatchOperation.DownloadingFiles:
                    var downloadResult = await governmentHttpClient.DownloadCurrentBatch(batch, cancellationToken);
                    if (downloadResult.IsFailure)
                        return downloadResult;
                    break;

                case EBatchOperation.PendingGovernmentBatch:
                    if (!batch.RetryDate.HasValue)
                        return Result.Failure("The batch is missing the retry date.");

                    await Task.Delay(DateTime.Now - batch.RetryDate.Value, cancellationToken);
                    break;

                case EBatchOperation.ExtractingFiles:
                    var extractionResult = await fileExtractionService.ExtractFiles(batch, cancellationToken);
                    if (extractionResult.IsFailure)
                        return extractionResult;
                    break;

                case EBatchOperation.ProcessingCSVFiles:
                    var processingResult = await csvProcessingService.ProcessCsvFiles(batch, cancellationToken);
                    if (processingResult.IsFailure)
                        return processingResult;
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
}
