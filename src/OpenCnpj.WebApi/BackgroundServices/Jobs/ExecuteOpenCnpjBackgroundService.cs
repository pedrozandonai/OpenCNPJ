using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Repositories;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Government.Clients.Interfaces;
using OpenCnpj.Core.Configurations;
using OpenCnpj.WebApi.BackgroundServices.Abstractions;

namespace OpenCnpj.WebApi.BackgroundServices.Jobs;

//TODO: Ver do pq ta dando pau quando executa de novo. System.InvalidOperationException: The transaction object is not associated with the same connection object as this command
public class ExecuteOpenCnpjBackgroundService(IOptions<BackgroundJobSettings> options, IServiceProvider serviceProvider, Serilog.ILogger logger) 
    : BackgroundJob(options.Value, serviceProvider, logger)
{
    protected override async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        var batchService = scope.ServiceProvider.GetRequiredService<IBatchService>();
        var batchRepository = scope.ServiceProvider.GetRequiredService<IBatchRepository>();
        var governmentHttpClient = scope.ServiceProvider.GetRequiredService<IGovernmentHttpClient>();
        var fileExtractionService = scope.ServiceProvider.GetRequiredService<IFileExtractionService>();
        var csvProcessingService = scope.ServiceProvider.GetRequiredService<ICsvProcessingService>();

        var batchIdentifier = DateTime.Now.ToString("yyyy-MM");

        var batch = await batchRepository.GetBatchByIdentifier(batchIdentifier, cancellationToken);
        if (batch == null)
        {
            var batchCreationResult = await batchService.CreateNewBatch(batchIdentifier, cancellationToken);
            if (batchCreationResult.IsFailure)
                return batchCreationResult;

            batch = batchCreationResult.Value;
        }

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
