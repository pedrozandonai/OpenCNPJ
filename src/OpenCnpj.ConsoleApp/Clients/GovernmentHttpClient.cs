using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Clients.Interfaces;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Constants;
using Serilog;
using System.Text.RegularExpressions;

namespace OpenCnpj.ConsoleApp.Clients;

public class GovernmentHttpClient(HttpClient httpClient, GovSetttings govSetttings, IBatchService batchService, ILogger logger) : IGovernmentHttpClient
{
    public async Task<Result> DownloadCurrentBatch(Batch batch, CancellationToken cancellationToken)
    {
        try
        {
            var updateBatchResult = await batchService.UpdateBatchStatus(batch, "Downloading files", cancellationToken);
            if (updateBatchResult.IsFailure)
                return Result.Failure(updateBatchResult.Error);

            string currentGovDataUrl = string.Format("{0}/{1}", govSetttings.BaseUrl, batch.Identifier);

            logger.Information("Fetching download URLs from: {0}", currentGovDataUrl);

            var response = await httpClient.GetAsync(currentGovDataUrl, cancellationToken);
            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var currentCsvUrlsGovData = ExtractDownloadUrls(htmlContent, currentGovDataUrl);

            logger.Information("Found {0} URL's for downloading.", currentCsvUrlsGovData.Count);

            await DownloadFiles(currentCsvUrlsGovData, batch, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "An error occurred while downloading the government files.";

            logger.Error(ex, errorMessage);

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
        var threadAmount = Environment.ProcessorCount;

        if (threadAmount > urls.Count)
            threadAmount = urls.Count;

        var semaphore = new SemaphoreSlim(threadAmount);

        var tasks = urls.Select(async url =>
        {
            await semaphore.WaitAsync();
            try
            {
                var rawDirectory = Paths.GetRawDirectoryByBatch(batch);
                if (!Directory.Exists(rawDirectory))
                    Directory.CreateDirectory(rawDirectory);

                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(rawDirectory, fileName);

                logger.Information("Downloading: {0}", fileName);

                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;
                var buffer = new byte[81920]; // 80KB (bom tamanho para streaming)
                long totalRead = 0;
                int read;

                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = File.Create(filePath);

                var lastLoggedMb = 0;

                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, read);
                    totalRead += read;

                    var downloadedMb = (int)(totalRead / 1024 / 1024);
                    if (downloadedMb >= lastLoggedMb + 10) // log a cada 10MB
                    {
                        logger.Information("Downloading {0}: {1:N0} MB of {2:N0} MB",
                            fileName,
                            downloadedMb,
                            totalBytes.HasValue ? totalBytes.Value / 1024 / 1024 : -1);
                        lastLoggedMb = downloadedMb;
                    }
                }

                logger.Information("Download concluded: {0} ({1:N0} MB)",
                    fileName,
                    totalRead / 1024 / 1024);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while downloading the URL: {0}", url);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        return Result.Success();
    }
}

