using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Government.Clients.Interfaces;
using OpenCnpj.Core.Configurations;
using Polly;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Government.Clients;

public class GovernmentHttpClient(IBatchService batchService, IBatchFileService batchFileService, HttpClient httpClient, GovSetttings govSetttings, TweakSettings tweakSettings, ILogger logger) : IGovernmentHttpClient
{
    private readonly ILogger _logger = logger.ForContext<IGovernmentHttpClient>();

    public async Task<Result> DownloadCurrentBatch(Batch batch, Channel<BatchFile> downloadedFilesChannel, CancellationToken cancellationToken)
    {
        try
        {
            string currentGovDataUrl = string.Format("{0}/{1}", govSetttings.BaseUrl, batch.Period);

            _logger.Information("Fetching download URLs from: {0}", currentGovDataUrl);

            var response = await httpClient.GetAsync(currentGovDataUrl, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var updateResult = await batchService.UpdateBatch(batch, batch.SetPendingGovernmentBatch, cancellationToken);
                if (updateResult.IsFailure)
                    return updateResult;

                _logger.Information("The data from the government could not be found on this batch date: {0}, the next fetch will be at {1}",
                    batch.Period, batch.RetryDate!.Value);

                downloadedFilesChannel.Writer.Complete();

                return Result.Success();
            }

            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var currentCsvUrlsGovData = ExtractDownloadUrls(htmlContent, currentGovDataUrl);

            _logger.Information("Found {0} URL's for downloading.", currentCsvUrlsGovData.Count);

            var partialDownloadBatchFiles = await CreatePartialBatchFiles(batch, currentCsvUrlsGovData, cancellationToken);

            await DownloadBatchFiles(partialDownloadBatchFiles, downloadedFilesChannel, cancellationToken);

            // Sinaliza que não há mais downloads
            downloadedFilesChannel.Writer.Complete();

            return Result.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "An error occurred while downloading the government files.";

            _logger.Error(ex, errorMessage);

            return Result.Failure(errorMessage);
        }
    }

    private async Task<IEnumerable<BatchFile>> CreatePartialBatchFiles(Batch batch, List<string> urls, CancellationToken cancellationToken)
    {
        List<BatchFile> batchFiles = [];

        foreach (var url in urls)
        {
            var rawDirectory = batch.GetRawDirectoryByBatch();
            if (!Directory.Exists(rawDirectory))
                Directory.CreateDirectory(rawDirectory);

            var fileName = Path.GetFileName(url);
            var filePath = Path.Combine(rawDirectory, fileName);
            var tempFilePath = filePath + ".partial";

            var partialDownloadBatchFileCreationResult = await batchFileService.CreatePartialDownloadBatchFile(batch.ID, url, tempFilePath, cancellationToken);
            if (partialDownloadBatchFileCreationResult.IsFailure)
            {
                _logger
                    .ForContext("batch", batch, true)
                    .ForContext("url", url, false)
                    .ForContext("fileName", fileName, false)
                    .ForContext("filePath", filePath, false)
                    .ForContext("tempFilePath", tempFilePath, false)
                    .Error("An error occurred while trying to create a new partial download batch file. Error: {0}", partialDownloadBatchFileCreationResult.Error);
                continue;
            }

            batchFiles.Add(partialDownloadBatchFileCreationResult.Value);
        }

        return batchFiles;
    }

    private static List<string> ExtractDownloadUrls(string htmlContent, string baseUrl)
    {
        var urls = new List<string>();

        var pattern = @"<a\s+href=""([^""]+\.zip)"">([^<]+)</a>";
        var matches = Regex.Matches(htmlContent, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            var href = match.Groups[1].Value;

            if (href.Contains("..") || href.Contains("Parent"))
                continue;

            var fullUrl = $"{baseUrl}/{href}";
            urls.Add(fullUrl);
        }

        return urls;
    }

    public async Task<Result> DownloadBatchFiles(IEnumerable<BatchFile> batchFiles, Channel<BatchFile> downloadedFilesChannel, CancellationToken cancellationToken)
    {
        if (tweakSettings.DownloadSettings.AmountAtTheSameTime > batchFiles.Count())
        {
            _logger.Warning("The amount of downloads at the same time is higher than the downloads itself. lowering to match the amount of downloads.");
            tweakSettings.DownloadSettings.AmountAtTheSameTime = batchFiles.Count();
        }

        var retryPolicy = Policy
            .Handle<Exception>()
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: tweakSettings.DownloadSettings.RetryFailureDownloadsAmount,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, tweakSettings.DownloadSettings.ExponencialSecondsIntervalBetweenRetries)),
                onRetry: (ex, ts) =>
                {
                    _logger.Warning("Retry after {0}s due to {1}", ts.TotalSeconds, ex.Message);
                }
            );

        var semaphore = new SemaphoreSlim(tweakSettings.DownloadSettings.AmountAtTheSameTime);

        var tasks = batchFiles.Select(async batchFile =>
        {
            BatchFile? partialDownloadedBatchFile = null;

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var downloadAndCreateBatchFileResult = await DownloadAndCreateBatchFile(batchFile, cancellationToken);
                    if (downloadAndCreateBatchFileResult.IsFailure)
                        return;

                    await downloadedFilesChannel.Writer.WriteAsync(downloadAndCreateBatchFileResult.Value, cancellationToken);
                });
            }
            catch (Exception ex)
            {
                _logger
                .ForContext("batchFile", batchFile, true)
                .Error(ex, "Error while downloading the batch file {0} with url: {1}", batchFile.FileName, batchFile.Url);

                if (partialDownloadedBatchFile == null)
                    return;

                var updateBatchFileResult = await batchFileService.UpdateBatchFile(partialDownloadedBatchFile, () => partialDownloadedBatchFile.SetOperationFailure(ex.Message), cancellationToken);
                if (updateBatchFileResult.IsFailure)
                {
                    _logger
                        .ForContext("batchFile", partialDownloadedBatchFile, true)
                        .Error("An error ocurred while tryingg to update the batch file status. Error: {0}", updateBatchFileResult.Error);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        return Result.Success();
    }

    private async Task<Result<BatchFile>> DownloadAndCreateBatchFile(BatchFile partialBatchFile, CancellationToken cancellationToken)
    {
        var downloadPartialBatchFileResult = await DownloadPartialBatchFile(partialBatchFile, cancellationToken);
        if (downloadPartialBatchFileResult.IsFailure)
            return Result.Failure<BatchFile>(downloadPartialBatchFileResult.Error);

        var downloadBatchFileCreationResult = await batchFileService.CreateDownloadBatchFileByPartialDownloadedBatchFile(downloadPartialBatchFileResult.Value, cancellationToken);
        if (downloadBatchFileCreationResult.IsFailure)
            return Result.Failure<BatchFile>(downloadBatchFileCreationResult.Error);

        return Result.Success(downloadBatchFileCreationResult.Value);
    }

    private async Task<Result<BatchFile>> DownloadPartialBatchFile(BatchFile batchFile, CancellationToken cancellationToken)
    {
        long existingLength = 0;
        if (File.Exists(batchFile.FilePath))
            existingLength = new FileInfo(batchFile.FilePath).Length;

        using var request = new HttpRequestMessage(HttpMethod.Get, batchFile.Url);
        if (existingLength > 0)
            request.Headers.Range = new RangeHeaderValue(existingLength, null);

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
        {
            _logger.Warning("Range is invalid for {0}, restarting.", batchFile.FileName);
            File.Delete(batchFile.FilePath);

            using var freshResponse = await httpClient.GetAsync(batchFile.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            freshResponse.EnsureSuccessStatusCode();

            await SaveToFile(freshResponse, batchFile.FilePath, 0, cancellationToken);
        }
        else
        {
            response.EnsureSuccessStatusCode();
            await SaveToFile(response, batchFile.FilePath, existingLength, cancellationToken);
        }

        var updateBatchFileResult = await batchFileService.UpdateBatchFile(batchFile, batchFile.SetDownloadCompleted, cancellationToken);
        if (updateBatchFileResult.IsFailure)
        {
            _logger
                .ForContext("batchFile", batchFile, true)
                .Error("An error ocurred while trying to update the batch file status. Error: {0}", updateBatchFileResult.Error);

            return Result.Failure<BatchFile>("An error ocurred while trying to update the batch file status");
        }

        _logger
            .ForContext("partialDownloadBatchFile", batchFile, true)
            .Information("Successfully downloaded file {0}", batchFile.FileName);

        return Result.Success(batchFile);
    }

    private async Task SaveToFile(HttpResponseMessage response, string filePath, long existingLength, CancellationToken cancellationToken)
    {
        var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength + existingLength : null;

        var buffer = new byte[81920];
        long totalRead = existingLength;
        int read;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);

        var lastLoggedMb = (int)(existingLength / 1024 / 1024);

        while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            totalRead += read;

            var downloadedMb = (int)(totalRead / 1024 / 1024);
            if (downloadedMb >= lastLoggedMb + tweakSettings.DownloadSettings.MbAmountToLog)
            {
                _logger.Information("Downloading {0}... {1:N0} mb of {2:N0} mb",
                    Path.GetFileNameWithoutExtension(filePath),
                    downloadedMb,
                    totalBytes.HasValue ? totalBytes.Value / 1024 / 1024 : -1);
                lastLoggedMb = downloadedMb;
            }
        }
    }
}

