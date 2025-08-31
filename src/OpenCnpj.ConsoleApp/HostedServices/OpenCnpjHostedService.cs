using CSharpFunctionalExtensions;
using Microsoft.Extensions.Hosting;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Clients.Interfaces;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
using OpenCnpj.ConsoleApp.Services.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.HostedServices;

public class OpenCnpjHostedService(IBatchService batchService, IGovernmentHttpClient governmentHttpClient, IFileExtractionService fileExtractionService, ICsvProcessingService csvProcessingService, IFormatDataService formatDataService, ILogger logger, IHostApplicationLifetime lifetime) : BackgroundService
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
        // Criar batch
        var batch = await batchService.CreateNewBatch(cancellationToken);
        if (batch.IsFailure)
            return batch;

        var currentBatch = batch.Value;

        try
        {
             //1. Download dos arquivos
            var downloadResult = await governmentHttpClient.DownloadCurrentBatch(currentBatch, cancellationToken);
            if (downloadResult.IsFailure)
                return downloadResult;

            // 2. Extração dos arquivos
            var extractionResult = await fileExtractionService.ExtractFiles(
                currentBatch, cancellationToken);
            if (extractionResult.IsFailure)
                return extractionResult;

            // 3. Processamento dos dados RAW
            var processingResult = await csvProcessingService.ProcessCsvFiles(
                currentBatch, cancellationToken);
            if (processingResult.IsFailure)
                return processingResult;

            // 4. Formatar os dados raw do mongo para postgres
            var formattingResult = await formatDataService.FormatData(cancellationToken);
            if (formattingResult.IsFailure)
                return formattingResult;

            // 4. Marcar batch como concluído
            //await _batchService.CompleteBatchAsync(currentBatch.Id, cancellationToken);

            _logger.Information("Batch {0} completed successfully", currentBatch.ID);
            return Result.Success();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
