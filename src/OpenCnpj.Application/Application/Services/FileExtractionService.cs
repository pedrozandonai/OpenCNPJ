using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using System.IO.Compression;
using System.Threading.Channels;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services;

public class FileExtractionService(IBatchService batchService, IBatchFileService batchFileService, ILogger logger) : IFileExtractionService
{
    private readonly ILogger _logger = logger.ForContext<FileExtractionService>();

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
                var setBatchFileOperationFailureResult = await batchService.UpdateBatch(batch, () => batch.SetOperationFailure(verificationResult.Error), cancellationToken);
                if (setBatchFileOperationFailureResult.IsFailure)
                    return setBatchFileOperationFailureResult;

                extractedFilesChannel.Writer.Complete();

                return verificationResult;
            }

            var extractedDirectory = batch.GetExtractedDirectoryByBatch();
            if (!Directory.Exists(extractedDirectory))
                Directory.CreateDirectory(extractedDirectory);

            int extractedCount = 0;

            await foreach (var batchFile in downloadedFilesChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var extractedFileResult = await ExtractZipFile(batchFile, extractedDirectory, cancellationToken);
                if (extractedFileResult.IsFailure)
                    continue;

                await extractedFilesChannel.Writer.WriteAsync(extractedFileResult.Value, cancellationToken);
                extractedCount++;
            }

            // Sinaliza que não há mais extrações
            extractedFilesChannel.Writer.Complete();

            _logger.Information("Extracted {0} files", extractedCount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error extracting files for batch {0}", batch.ID);
            return Result.Failure($"Error extracting files: {ex.Message}");
        }
    }

    private async Task<Result<BatchFile>> ExtractZipFile(BatchFile batchFile, string extractedDirectory, CancellationToken cancellationToken)
    {
        var setBatchFileStatusToExtractingResult = await batchFileService.UpdateBatchFile(batchFile, batchFile.StartExtractingFile, cancellationToken);
        if (setBatchFileStatusToExtractingResult.IsFailure)
            return Result.Failure<BatchFile>(setBatchFileStatusToExtractingResult.Error);

        var fileName = Path.GetFileName(batchFile.FilePath);
        var baseName = Path.GetFileNameWithoutExtension(fileName);

        return await Task.Run(async () =>
        {
            BatchFile? currentZipBatchFile = null;
            BatchFile? currentExtractedBatchFile = null;

            try
            {
                currentZipBatchFile = batchFile;

                logger.Information("Extracting: {0}", fileName);

                using (var archive = ZipFile.OpenRead(batchFile.FilePath))
                {
                    if (archive.Entries.Count == 0)
                    {
                        _logger.Warning("No files were found in zip: {0}", fileName);

                        return Result.Failure<BatchFile>("No files were found in zip.");
                    }

                    var entry = archive.Entries[0];
                    var targetPath = Path.Combine(extractedDirectory, baseName);

                    var extractedBatchFileCreationResult = await batchFileService.CreateExtractedBatchFile(batchFile, targetPath, cancellationToken);
                    if (extractedBatchFileCreationResult.IsFailure)
                    {
                        _logger.Error(extractedBatchFileCreationResult.Error);
                        return Result.Failure<BatchFile>(extractedBatchFileCreationResult.Error);
                    }

                    currentExtractedBatchFile = extractedBatchFileCreationResult.Value;

                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                    entry.ExtractToFile(targetPath);
                    _logger.Information("File extracted to {0}", targetPath);
                }

                var setZipBatchFileSuccessResult = await batchFileService.UpdateBatchFile(currentZipBatchFile, () => currentZipBatchFile.SetExtranctionCompleted(), cancellationToken);
                if (setZipBatchFileSuccessResult.IsFailure)
                {
                    _logger
                    .ForContext("zipBatchFile", currentZipBatchFile, true)
                    .Error("Unable to update the zip batch file operation status to success. Error: {0}", setZipBatchFileSuccessResult.Error);
                }

                _logger
                .ForContext("currentZipBatchFile", currentZipBatchFile, true)
                .ForContext("currentExtractedBatchFile", currentExtractedBatchFile, true)
                .Information("Successfully extracted file {0} and deleted zip file {1}", currentExtractedBatchFile.FileName, currentZipBatchFile.FileName);

                return Result.Success(currentExtractedBatchFile);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting zip file: {0}", fileName);

                if (currentZipBatchFile != null)
                {
                    var setZipBatchFileFailureResult = await batchFileService.UpdateBatchFile(currentZipBatchFile, () => currentZipBatchFile.SetOperationFailure(ex.Message), cancellationToken);
                    if (setZipBatchFileFailureResult.IsFailure)
                    {
                        _logger
                        .ForContext("zipBatchFile", currentZipBatchFile, true)
                        .Error(ex, "Unable to update the zip batch file operation status to failure. Error: {0}", setZipBatchFileFailureResult.Error);
                    }
                }

                if (currentExtractedBatchFile != null)
                {
                    var setExtractedBatchFileFailureResult = await batchFileService.UpdateBatchFile(currentExtractedBatchFile, () => currentExtractedBatchFile.SetOperationFailure(ex.Message), cancellationToken);
                    if (setExtractedBatchFileFailureResult.IsFailure)
                    {
                        _logger
                        .ForContext("extractedBatchFile", currentExtractedBatchFile, true)
                        .Error(ex, "Unable to update the extracted batch file operation status to failure. Error: {0}", setExtractedBatchFileFailureResult.Error);
                    }
                }

                return Result.Failure<BatchFile>("Error extracting zip file.");
            }
        }, cancellationToken);
    }
}