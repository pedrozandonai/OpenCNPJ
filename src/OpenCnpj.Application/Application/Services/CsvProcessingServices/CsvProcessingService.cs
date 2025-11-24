using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Core.Configurations;
using System.Globalization;
using System.Text;
using System.Threading.Channels;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices;
public class CsvProcessingService(
    IBatchFileService batchFileService,
    CsvStrategyFactory strategyFactory,
    TweakSettings tweakSettings,
    ILogger logger) : ICsvProcessingService
{
    private readonly ILogger _logger = logger.ForContext<CsvProcessingService>();

    public async Task<Result> ProcessCsvFilesIncremental(
        Batch batch,
        Channel<BatchFile> extractedFilesChannel,
        CancellationToken cancellationToken)
    {
        var config = CreateCsvConfiguration();

        int maxParallelFiles = tweakSettings.RawFilesProcessingSettings.FilesAtTheSameTimeAmount;
        var semaphore = new SemaphoreSlim(maxParallelFiles);

        var processingTasks = new List<Task<Result>>();

        await foreach (var extractedFile in extractedFilesChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await semaphore.WaitAsync(cancellationToken);

            var task = Task.Run(async () =>
            {
                try
                {
                    return await ProcessSingleFile(batch, extractedFile, config, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            processingTasks.Add(task);
        }

        var results = await Task.WhenAll(processingTasks);

        if (results.All(r => r.IsSuccess))
            return Result.Success();

        var errors = string.Join("; ", results.Where(r => !r.IsSuccess).Select(r => r.Error));

        return Result.Failure(errors);
    }

    private async Task<Result> ProcessSingleFile(Batch batch, BatchFile extractedBatchFile, CsvConfiguration config, CancellationToken cancellationToken)
    {
        try
        {
            var setProcessingBatchFileResult = await batchFileService.UpdateBatchFile(extractedBatchFile, extractedBatchFile.StartProcessingFile, cancellationToken);
            if (setProcessingBatchFileResult.IsFailure)
            {
                _logger
                    .ForContext("extractedBatchFile", extractedBatchFile, true)
                    .Error(setProcessingBatchFileResult.Error);

                return Result.Failure(setProcessingBatchFileResult.Error);
            }

            var strategy = strategyFactory.GetStrategy(extractedBatchFile.FileName);

            if (strategy == null)
            {
                var errorMessage = string.Format("No strategy found for file: {0}. Supported patterns: {1}",
                    extractedBatchFile.FileName, string.Join(", ", strategyFactory.GetSupportedFilePatterns()));

                _logger.Warning(errorMessage);

                setProcessingBatchFileResult = await batchFileService.UpdateBatchFile(extractedBatchFile, () => extractedBatchFile.SetOperationFailure(errorMessage), cancellationToken);
                if (setProcessingBatchFileResult.IsFailure)
                {
                    _logger
                        .ForContext("extractedBatchFile", extractedBatchFile, true)
                        .Error(setProcessingBatchFileResult.Error);

                    return Result.Failure(setProcessingBatchFileResult.Error);
                }

                return Result.Failure(errorMessage);
            }

            _logger.Information("Processing file {0} with strategy {1}", extractedBatchFile.FileName, strategy.GetType().Name);

            Encoding governmentEncoding = Encoding.Latin1;

            Result strategyProcessingResult;
            using (var reader = new StreamReader(extractedBatchFile.FilePath, governmentEncoding))
            {
                using var csv = new CsvReader(reader, config);
                strategyProcessingResult = await strategy.ProcessAsync(batch, csv, extractedBatchFile.FileName, cancellationToken);
            }

            var setFinishedProcessingBatchFileResult = await batchFileService.UpdateBatchFile(extractedBatchFile, extractedBatchFile.SetProcessingCompleted, cancellationToken);
            if (setFinishedProcessingBatchFileResult.IsFailure)
            {
                _logger
                    .ForContext("extractedBatchFile", extractedBatchFile, true)
                    .Error(setFinishedProcessingBatchFileResult.Error);

                return Result.Failure(setFinishedProcessingBatchFileResult.Error);
            }

            if (strategyProcessingResult.IsSuccess)
                _logger.Information("Successfully processed file: {0} and removed file {1}", extractedBatchFile.FileName, extractedBatchFile.FilePath);

            return strategyProcessingResult;
        }
        catch (Exception ex)
        {
            string errorMessage = string.Format("Unexpected error processing file: {0}", extractedBatchFile.FileName);

            _logger.Error(ex, errorMessage);

            var setProcessingBatchFileResult = await batchFileService.UpdateBatchFile(extractedBatchFile, () => extractedBatchFile.SetOperationFailure(errorMessage), cancellationToken);
            if (setProcessingBatchFileResult.IsFailure)
            {
                _logger
                    .ForContext("extractedBatchFile", extractedBatchFile, true)
                    .Error(setProcessingBatchFileResult.Error);

                return Result.Failure(setProcessingBatchFileResult.Error);
            }

            return Result.Failure(errorMessage);
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
