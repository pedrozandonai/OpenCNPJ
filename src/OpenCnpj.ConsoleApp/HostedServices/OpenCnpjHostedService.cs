using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenCnpj.ConsoleApp.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Repositories;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Clients.Interfaces;
using OpenCnpj.ConsoleApp.Helpers;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
using OpenCnpj.ConsoleApp.Services.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.HostedServices;

public class OpenCnpjHostedService(IBatchService batchService, IBatchRepository batchRepository, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IFormatDataService formatDataService, ILogger logger, IHostApplicationLifetime lifetime, IServiceProvider serviceProvider) : BackgroundService
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
            
             ////1. Download dos arquivos
             // if (batch.ApplicationLastStepID == EApplicationStep.StartedApplication)
             // {
             //     var downloadResult = await governmentHttpClient.DownloadCurrentBatch(batch, cancellationToken);
             //     if (downloadResult.IsFailure)
             //         return downloadResult;
             //
             //     updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.DownloadingFiles, cancellationToken);
             //     if (updateLastStepResult.IsFailure)
             //         return updateLastStepResult;
             // }

            //// 2. Extração dos arquivos
            // if (batch.ApplicationLastStepID == EApplicationStep.DownloadingFiles)
            // {
            //     var extractionResult = await fileExtractionService.ExtractFiles(
            //         batch, cancellationToken);
            //     if (extractionResult.IsFailure)
            //         return extractionResult;
            //
            //     updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ExtractingFiles, cancellationToken);
            //     if (updateLastStepResult.IsFailure)
            //         return updateLastStepResult;
            // }

            //// 3. Processamento dos dados RAW
            // if (batch.ApplicationLastStepID == EApplicationStep.ExtractingFiles)
            // {
                //var processingResult = await csvProcessingService.ProcessCsvFiles(
                //    batch, cancellationToken);
                //if (processingResult.IsFailure)
                //    return processingResult;
            
                //updateLastStepResult = await batchService.SetApplicationLastStep(batch, EApplicationStep.ProcessingRawFiles, cancellationToken);
                //if (updateLastStepResult.IsFailure)
                //    return updateLastStepResult;
            // }

            //// 4. Formatar os dados raw do mongo para postgres
            //if (batch.ApplicationLastStepID == EApplicationStep.ProcessingRawFiles)
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

            _logger.Information("Batch {0} completed successfully", batch.ID);
            return Result.Success();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
