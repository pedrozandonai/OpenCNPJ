using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Constants;
using OpenCnpj.ConsoleApp.Mappers;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Factory;
using Serilog;
using System.Globalization;

namespace OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
public class CsvProcessingService(CsvStrategyFactory strategyFactory, IBatchService batchService, ILogger logger) : ICsvProcessingService
{
    public async Task<Result> ProcessCsvFiles(Batch batch, CancellationToken cancellationToken)
    {
        var updateBatchResult = await batchService.UpdateBatchStatus(batch, "Processing CSV files", cancellationToken);
        if (updateBatchResult.IsFailure)
            return Result.Failure(updateBatchResult.Error);

        var extractedDirectory = Paths.GetExtractedDirectoryByBatch(batch);

        if (!Directory.Exists(extractedDirectory))
        {
            return Result.Failure($"Extracted directory not found: {extractedDirectory}");
        }

        var extractedFiles = Directory.GetFiles(extractedDirectory, "*", SearchOption.AllDirectories);

        if (extractedFiles.Length == 0)
        {
            logger.Warning("No CSV files found in {0}", extractedDirectory);
            return Result.Success();
        }

        logger.Information("Found {0} CSV files to process", extractedFiles.Length);

        var config = CreateCsvConfiguration();

        const int maxParallelFiles = 10;
        var semaphore = new SemaphoreSlim(maxParallelFiles);

        var tasks = extractedFiles.Select(async extractedFile =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await ProcessSingleFile(batch, extractedFile, config, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(tasks);

        if (results.All(r => r.IsSuccess))
            return Result.Success();

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));

        return Result.Failure(errors);
    }

    public async Task<Result> ProcessSingleFile(Batch batch, string filePath, CsvConfiguration config, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(filePath);

        try
        {
            var strategy = strategyFactory.GetStrategy(fileName);

            if (strategy == null)
            {
                logger.Warning("No strategy found for file: {0}. Supported patterns: {1}",
                    fileName, string.Join(", ", strategyFactory.GetSupportedFilePatterns()));
                return Result.Success(); // Não é um erro crítico, apenas pula o arquivo
            }

            logger.Information("Processing file {0} with strategy {1}", fileName, strategy.GetType().Name);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            var result = await strategy.ProcessAsync(batch, csv, fileName, cancellationToken);

            if (result.IsSuccess)
                logger.Information("Successfully processed file: {0}", fileName);

            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unexpected error processing file: {1}", fileName);


            return Result.Failure($"Unexpected error processing {fileName}: {ex.Message}");
        }
    }

    private static CsvConfiguration CreateCsvConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ";",
            BadDataFound = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
            Mode = CsvMode.RFC4180,
            DetectColumnCountChanges = false,
            Quote = '"',
        };
    }
}
