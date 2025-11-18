using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Core.Configurations;
using System.Globalization;
using System.Text;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices;
public class CsvProcessingService(IBatchService batchService, CsvStrategyFactory strategyFactory, TweakSettings tweakSettings, ILogger logger) : ICsvProcessingService
{
    public async Task<Result> ProcessCsvFiles(Batch batch, CancellationToken cancellationToken)
    {
        var startProcessingCsvFilesResult = await batchService.UpdateBatch(batch, batch.StartProcessingCsvFiles, cancellationToken);
        if (startProcessingCsvFilesResult.IsFailure)
            return startProcessingCsvFilesResult;

        var extractedDirectory = batch.GetExtractedDirectoryByBatch();

        Result setOperationFailureResult;
        Result updateBatchResult;

        if (!Directory.Exists(extractedDirectory))
        {
            string errorMessage = string.Format("Extracted directory not found: {0}", extractedDirectory);

            setOperationFailureResult = batch.SetOperationFailure(errorMessage);
            if (setOperationFailureResult.IsFailure)
                return setOperationFailureResult;

            updateBatchResult = await batchService.UpdateBatch(batch, () => { return setOperationFailureResult; }, cancellationToken);
            if (updateBatchResult.IsFailure)
                return updateBatchResult;

            return Result.Failure(errorMessage);
        }

        var extractedFiles = Directory.GetFiles(extractedDirectory, "*", SearchOption.AllDirectories);

        if (extractedFiles.Length == 0)
        {
            string errorMessage = string.Format("No CSV files found in {0}", extractedDirectory);

            setOperationFailureResult = batch.SetOperationFailure(errorMessage);
            if (setOperationFailureResult.IsFailure)
                return setOperationFailureResult;

            updateBatchResult = await batchService.UpdateBatch(batch, () => { return setOperationFailureResult; }, cancellationToken);
            if (updateBatchResult.IsFailure)
                return updateBatchResult;

            return Result.Failure(errorMessage);
        }

        logger.Information("Found {0} CSV files to process", extractedFiles.Length);

        var config = CreateCsvConfiguration();

        int maxParallelFiles = tweakSettings.RawFilesProcessingSettings.FilesAtTheSameTimeAmount;
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
        {
            updateBatchResult = await batchService.UpdateBatch(batch, batch.SetOperationSuccess, cancellationToken);
            if (updateBatchResult.IsFailure)
                return updateBatchResult;

            return Result.Success();
        }

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));

        setOperationFailureResult = batch.SetOperationFailure(errors);
        if (setOperationFailureResult.IsFailure)
            return setOperationFailureResult;

        updateBatchResult = await batchService.UpdateBatch(batch, () => { return setOperationFailureResult; }, cancellationToken);
        if (updateBatchResult.IsFailure)
            return updateBatchResult;

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

            Encoding governmentEncoding = Encoding.Latin1;

            using var reader = new StreamReader(filePath, governmentEncoding);
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
