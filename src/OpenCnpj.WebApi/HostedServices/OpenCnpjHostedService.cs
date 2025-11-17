using CSharpFunctionalExtensions;
using OpenCnpj.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Repositories;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.WebApi.Clients.Interfaces;
using OpenCnpj.WebApi.Services.CsvProcessingServices;
using OpenCnpj.WebApi.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.WebApi.HostedServices;

public class OpenCnpjHostedService(IServiceProvider serviceProvider, ILogger logger) : IHostedService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjHostedService>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var result = await RunProcessing(scope.ServiceProvider, cancellationToken);

            if (result.IsFailure)
            {
                _logger.Error("Processing failed: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Unhandled exception occurred");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task<Result> RunProcessing(IServiceProvider localServiceProvider, CancellationToken cancellationToken)
    {
        var batchService = localServiceProvider.GetRequiredService<IBatchService>();
        var batchRepository = localServiceProvider.GetRequiredService<IBatchRepository>();
        var governmentHttpClient = localServiceProvider.GetRequiredService<IGovernmentHttpClient>();
        var fileExtractionService = localServiceProvider.GetRequiredService<IFileExtractionService>();
        var csvProcessingService = localServiceProvider.GetRequiredService<ICsvProcessingService>();

        var batchIdentifier = DateTime.Now.ToString("yyyy-MM");

        var batch = await batchRepository.GetBatchByIdentifier(batchIdentifier, cancellationToken);
        if (batch == null)
        {
            var batchCreationResult = await batchService.CreateNewBatch(batchIdentifier, cancellationToken);
            if (batchCreationResult.IsFailure)
                return batchCreationResult;

            batch = batchCreationResult.Value;
        }

        try
        {
            Result updateLastStepResult;

            //1.Download dos arquivos
            if (batch.ApplicationLastStepID == EApplicationStep.StartedApplication)
            {
                var downloadResult = await governmentHttpClient.DownloadCurrentBatch(batch, cancellationToken);
                if (downloadResult.IsFailure)
                    return downloadResult;

                updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.DownloadingFiles, cancellationToken);
                if (updateLastStepResult.IsFailure)
                    return updateLastStepResult;
            }

            //2.Extração dos arquivos
            if (batch.ApplicationLastStepID == EApplicationStep.DownloadingFiles)
            {
                var extractionResult = await fileExtractionService.ExtractFiles(
                    batch, cancellationToken);
                if (extractionResult.IsFailure)
                    return extractionResult;

                updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ExtractingFiles, cancellationToken);
                if (updateLastStepResult.IsFailure)
                    return updateLastStepResult;
            }

            //3.Processamento dos dados RAW
            if (batch.ApplicationLastStepID == EApplicationStep.ExtractingFiles)
            {
                var processingResult = await csvProcessingService.ProcessCsvFiles(
                    batch, cancellationToken);
                if (processingResult.IsFailure)
                    return processingResult;

                updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ProcessingRawFiles, cancellationToken);
                if (updateLastStepResult.IsFailure)
                    return updateLastStepResult;
            }

            _logger.Information("Batch {0} completed successfully", batch.ID);
            return Result.Success();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
