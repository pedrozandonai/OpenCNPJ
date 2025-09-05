using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenCnpj.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Clients.Interfaces;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
using OpenCnpj.ConsoleApp.Services.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.HostedServices;

public class OpenCnpjHostedService(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime, ILogger logger) : BackgroundService
{
    private readonly ILogger _logger = logger.ForContext<OpenCnpjHostedService>();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var result = await RunProcessing(stoppingToken);

            if (result.IsFailure)
            {
                _logger.Error("Processing failed: {Error}", result.Error);
                Environment.ExitCode = 1;
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Unhandled exception occurred");
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }

    private async Task<Result> RunProcessing(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var batchService = scope.ServiceProvider.GetRequiredService<IBatchService>();
        var batchRepository = scope.ServiceProvider.GetRequiredService<IBatchRepository>();
        var governmentHttpClient = scope.ServiceProvider.GetRequiredService<IGovernmentHttpClient>();
        var fileExtractionService = scope.ServiceProvider.GetRequiredService<IFileExtractionService>();
        var csvProcessingService = scope.ServiceProvider.GetRequiredService<ICsvProcessingService>();
        var formatDataService= scope.ServiceProvider.GetRequiredService<IFormatDataService>();

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

            //1. Download dos arquivos
            //if (batch.ApplicationLastStepId == EApplicationStep.StartedApplication)
            //{
            //var downloadResult = await governmentHttpClient.DownloadCurrentBatch(batch, cancellationToken);
            //if (downloadResult.IsFailure)
            //    return downloadResult;

            //updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.DownloadingFiles, cancellationToken);
            //if (updateLastStepResult.IsFailure)
            //    return updateLastStepResult;
            //}

            // 2. Extração dos arquivos
            //if (batch.ApplicationLastStep == EApplicationStep.DownloadingFiles)
            //{
            //var extractionResult = await fileExtractionService.ExtractFiles(
            //        batch, cancellationToken);
            //    if (extractionResult.IsFailure)
            //        return extractionResult;

            //    updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ExtractingFiles, cancellationToken);
            //    if (updateLastStepResult.IsFailure)
            //        return updateLastStepResult;
            //}

            // 3. Processamento dos dados RAW
            //if (batch.ApplicationLastStep == EApplicationStep.ExtractingFiles)
            //{
                //var processingResult = await csvProcessingService.ProcessCsvFiles(batch, cancellationToken);
                //if (processingResult.IsFailure)
                //    return processingResult;

                //updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ProcessingRawFiles, cancellationToken);
                //if (updateLastStepResult.IsFailure)
                //    return updateLastStepResult;
            //}

            // 4. Formatar os dados raw do mongo para postgres
            //if (batch.ApplicationLastStepId == EApplicationStep.ProcessingRawFiles)
            //{
                var formattingResult = await formatDataService.FormatData(cancellationToken);
                if (formattingResult.IsFailure)
                    return formattingResult;
            
                updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.FormattingRawData, cancellationToken);
                if (updateLastStepResult.IsFailure)
                    return updateLastStepResult;
            //}

            // 4. Marcar batch como concluído
            //await _batchService.CompleteBatchAsync(batch.Id, cancellationToken);

            _logger.Information("Batch {0} completed successfully", batch.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
