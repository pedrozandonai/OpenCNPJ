using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Government.Clients.Interfaces;
using OpenCnpj.Core.Configurations;
using Polly;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Government.Clients;

public class GovernmentHttpClient(IBatchService batchService, HttpClient httpClient, GovSetttings govSetttings, TweakSettings tweakSettings, ILogger logger) : IGovernmentHttpClient
{
    private readonly ILogger _logger = logger.ForContext<IGovernmentHttpClient>();

    public async Task<Result> DownloadCurrentBatch(Batch batch, CancellationToken cancellationToken)
    {
        try
        {
            var updateResult = await batchService.UpdateBatch(batch, batch.StartDownloading, cancellationToken);
            if (updateResult.IsFailure)
                return updateResult;

            string currentGovDataUrl = string.Format("{0}/{1}", govSetttings.BaseUrl, batch.Identifier);

            _logger.Information("Fetching download URLs from: {0}", currentGovDataUrl);

            var response = await httpClient.GetAsync(currentGovDataUrl, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                updateResult = await batchService.UpdateBatch(batch, batch.SetPendingGovernmentBatch, cancellationToken);
                if (updateResult.IsFailure)
                    return updateResult;

                _logger.Information("The data from the government could not be found on this batch date: {0}, the next fetch will be at {1}", batch.Identifier, batch.RetryDate!.Value); 

                return Result.Success();
            }

            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var currentCsvUrlsGovData = ExtractDownloadUrls(htmlContent, currentGovDataUrl);

            _logger.Information("Found {0} URL's for downloading.", currentCsvUrlsGovData.Count);

            await DownloadFiles(currentCsvUrlsGovData, batch, cancellationToken);

            var setOperationSuccessResult = await batchService.UpdateBatch(batch, batch.SetOperationSuccess, cancellationToken);
            if (setOperationSuccessResult.IsFailure)
                return setOperationSuccessResult;

            return Result.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "An error occurred while downloading the government files.";

            _logger.Error(ex, errorMessage);

            return Result.Failure(errorMessage);
        }
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

    private async Task<Result> DownloadFiles(List<string> urls, Batch batch, CancellationToken cancellationToken)
    {
        if (tweakSettings.DownloadSettings.AmountAtTheSameTime > urls.Count)
        {
            _logger.Warning("The amount of downloads at the same time is higher than the downloads itself. lowering to match the amount of downloads.");
            tweakSettings.DownloadSettings.AmountAtTheSameTime = urls.Count;
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

        var tasks = urls.Select(async url =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var rawDirectory = batch.GetRawDirectoryByBatch();
                    if (!Directory.Exists(rawDirectory))
                        Directory.CreateDirectory(rawDirectory);

                    var fileName = Path.GetFileName(url);
                    var filePath = Path.Combine(rawDirectory, fileName);
                    var tempFilePath = filePath + ".partial";

                    long existingLength = 0;
                    if (File.Exists(tempFilePath))
                        existingLength = new FileInfo(tempFilePath).Length;

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (existingLength > 0)
                        request.Headers.Range = new RangeHeaderValue(existingLength, null);

                    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                    if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                    {
                        _logger.Warning("Range is invalid for {0}, restarting.", fileName);
                        File.Delete(tempFilePath);

                        using var freshResponse = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                        freshResponse.EnsureSuccessStatusCode();

                        await SaveToFile(freshResponse, tempFilePath, fileName, 0, cancellationToken);
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                        await SaveToFile(response, tempFilePath, fileName, existingLength, cancellationToken);
                    }

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    File.Move(tempFilePath, filePath);

                    _logger.Information("Download finished: {0}", fileName);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while downloading the url: {0}", url);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        return Result.Success();
    }

    private async Task SaveToFile(HttpResponseMessage response, string filePath, string fileName, long existingLength, CancellationToken cancellationToken)
    {
        var totalBytes = response.Content.Headers.ContentLength.HasValue
            ? response.Content.Headers.ContentLength + existingLength
            : null;

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
                    fileName,
                    downloadedMb,
                    totalBytes.HasValue ? totalBytes.Value / 1024 / 1024 : -1);
                lastLoggedMb = downloadedMb;
            }
        }
    }
}

