using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using ILogger = Serilog.ILogger;
using System.IO.Compression;
using OpenCnpj.WebApi.Services.Interfaces;

namespace OpenCnpj.WebApi.Services;

public class FileExtractionService(IBatchService batchService, ILogger logger) : IFileExtractionService
{
    public async Task<Result> ExtractFiles(Batch batch, CancellationToken cancellationToken)
    {
        try
        {
            var updateBatchResult = await batchService.UpdateBatchStatus(batch, "Extracting files", cancellationToken);
            if (updateBatchResult.IsFailure)
                return Result.Failure(updateBatchResult.Error);

            var verificationResult = batch.VerifyIfDirectoryExists();
            if (verificationResult.IsFailure)
                return verificationResult;

            var extractedDirectory = batch.GetExtractedDirectoryByBatch();

            if (!Directory.Exists(extractedDirectory))
                Directory.CreateDirectory(extractedDirectory);

            var zipFiles = Directory.GetFiles(Path.Combine(batch.Directory!, "raw"), "*.zip");

            if (zipFiles.Length == 0)
                return Result.Failure("No ZIP files found to extract");

            logger.Information("Found {0} ZIP files to extract", zipFiles.Length);

            var tasks = zipFiles.Select(async (zipFile) =>
            {
                await ExtractZipFile(zipFile, extractedDirectory, cancellationToken);
            });

            await Task.WhenAll(tasks);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error extracting files for batch {0}", batch.ID);

            return Result.Failure($"Error extracting files: {ex.Message}");
        }
    }

    private async Task ExtractZipFile(string zipFile, string extractedDirectory, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(zipFile);
        var baseName = Path.GetFileNameWithoutExtension(fileName);

        await Task.Run(() =>
        {
            logger.Information("Extracting: {0}", fileName);

            using (var archive = ZipFile.OpenRead(zipFile))
            {
                if (archive.Entries.Count == 0)
                {
                    logger.Warning("No files were found in zip: {0}", fileName);
                    return;
                }

                var entry = archive.Entries[0];
                var targetPath = Path.Combine(extractedDirectory, baseName);

                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                entry.ExtractToFile(targetPath);

                logger.Information("File extracted to {0}", targetPath);
            }

            File.Delete(zipFile);
            logger.Information("File {0} deleted.", fileName);
        }, cancellationToken);
    }
}
