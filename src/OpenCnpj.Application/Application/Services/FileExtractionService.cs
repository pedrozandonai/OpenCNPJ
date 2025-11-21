using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using System.IO.Compression;
using System.Threading.Channels;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services;

public class FileExtractionService(IBatchFileService batchService, ILogger logger) : IFileExtractionService
{
    public async Task<Result> ExtractFilesIncremental(
        Batch batch,
        Channel<BatchFile> downloadedFilesChannel,
        Channel<BatchFile> extractedFilesChannel,
        CancellationToken cancellationToken)
    {
        try
        {
            var verificationResult = batch.VerifyIfDirectoryExists();
            if (verificationResult.IsFailure)
            {
                var setOperationFailureResult = batch.SetOperationFailure(verificationResult.Error);
                if (setOperationFailureResult.IsFailure)
                    return setOperationFailureResult;

                var updateBatchResult = await batchService.UpdateBatch(batch, () => { return setOperationFailureResult; }, cancellationToken);
                if (updateBatchResult.IsFailure)
                    return updateBatchResult;

                extractedFilesChannel.Writer.Complete();

                return verificationResult;
            }

            var extractedDirectory = batch.GetExtractedDirectoryByBatch();
            if (!Directory.Exists(extractedDirectory))
                Directory.CreateDirectory(extractedDirectory);

            int extractedCount = 0;

            // Processa arquivos conforme eles chegam do canal de downloads
            await foreach (var batchFiile in downloadedFilesChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var extractedFile = await ExtractZipFile(batchFiile, extractedDirectory, cancellationToken);

                if (!string.IsNullOrEmpty(extractedFile))
                {
                    // Notifica que o arquivo foi extraído e está pronto para processamento
                    await extractedFilesChannel.Writer.WriteAsync(extractedFile, cancellationToken);
                    extractedCount++;
                }
            }

            // Sinaliza que não há mais extrações
            extractedFilesChannel.Writer.Complete();

            logger.Information("Extracted {0} files", extractedCount);

            var setOperationSuccessResult = await batchService.UpdateBatch(batch, batch.SetOperationSuccess, cancellationToken);
            if (setOperationSuccessResult.IsFailure)
                return setOperationSuccessResult;

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error extracting files for batch {0}", batch.ID);
            return Result.Failure($"Error extracting files: {ex.Message}");
        }
    }

    private async Task<string?> ExtractZipFile(BatchFile batchFile, string extractedDirectory, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(batchFile.FilePath);
        var baseName = Path.GetFileNameWithoutExtension(fileName);

        return await Task.Run(() =>
        {
            try
            {
                logger.Information("Extracting: {0}", fileName);

                using (var archive = ZipFile.OpenRead(batchFile.FilePath))
                {
                    if (archive.Entries.Count == 0)
                    {
                        logger.Warning("No files were found in zip: {0}", fileName);
                        return null;
                    }

                    var entry = archive.Entries[0];
                    var targetPath = Path.Combine(extractedDirectory, baseName);

                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                    entry.ExtractToFile(targetPath);
                    logger.Information("File extracted to {0}", targetPath);

                    File.Delete(batchFile.FilePath);
                    logger.Information("File {0} deleted.", fileName);

                    return targetPath;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error extracting zip file: {0}", fileName);
                return null;
            }
        }, cancellationToken);
    }
}