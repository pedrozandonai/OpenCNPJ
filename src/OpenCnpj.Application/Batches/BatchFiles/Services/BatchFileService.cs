using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Repositories;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Models.Enums;
using OpenCnpj.Application.Batches.BatchFiles.Queries;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;
using Serilog;

namespace OpenCnpj.Application.Batches.Batches.Services;

public class BatchFileService(IBatchRepository batchRepository, IBatchFileRepository batchFileRepository, IBatchFileQueries batchFileQueries, ILogger logger) : IBatchFileService
{
    private readonly ILogger _logger = logger.ForContext<BatchFileService>();

    public async Task<Result<BatchFile>> CreatePartialDownloadBatchFile(int batchID, string url, string filePath, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByID(batchID, cancellationToken);
        if (batch == null)
            return Result.Failure<BatchFile>("Unable to create batch file because the batch ID doesn't exists.");

        if (await batchFileQueries.BatchFileExistsByFilePath(filePath, cancellationToken))
            return Result.Failure<BatchFile>("One batch file already exists with the same file path.");

        var batchFile = BatchFile.CreatePartialDownloadBatchFile(batch.ID, url, filePath);

        var batchFileID = await batchFileRepository.Insert(batchFile, cancellationToken);

        var setIDResult = batchFile.SetID(batchFileID);
        if (setIDResult.IsFailure)
            return Result.Failure<BatchFile>(setIDResult.Error);

        return Result.Success(batchFile);
    }

    public async Task<Result<BatchFile>> CreateDownloadBatchFileByPartialDownloadedBatchFile(BatchFile partialDownloadedBatchFile, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByID(partialDownloadedBatchFile.BatchID, cancellationToken);
        if (batch == null)
            return Result.Failure<BatchFile>("Unable to create batch file because the batch ID doesn't exists.");

        var finalDownloadedFilePath = Path.Combine(batch.GetRawDirectoryByBatch(), Path.GetFileNameWithoutExtension(partialDownloadedBatchFile.FileName));

        if (await batchFileQueries.BatchFileExistsByFilePath(finalDownloadedFilePath, cancellationToken))
            return Result.Failure<BatchFile>("One batch file already exists with the same file path.");

        if (partialDownloadedBatchFile.Type != EBatchFileType.PartialDownloadedFile)
            return Result.Failure<BatchFile>("Unable to create a downloaded batch file from a non partial downloaded file.");

        if (File.Exists(finalDownloadedFilePath))
            File.Delete(finalDownloadedFilePath);

        File.Move(partialDownloadedBatchFile.FilePath, finalDownloadedFilePath);

        var batchFile = BatchFile.CreateDownloadedBatchFile(partialDownloadedBatchFile.ID, batch.ID, finalDownloadedFilePath);

        var batchFileID = await batchFileRepository.Insert(batchFile, cancellationToken);

        var setIDResult = batchFile.SetID(batchFileID);
        if (setIDResult.IsFailure)
            return Result.Failure<BatchFile>(setIDResult.Error);

        return Result.Success(batchFile);
    }

    public async Task<Result<BatchFile>> CreateExtractedBatchFile(BatchFile parentBatchFile, string filePath, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByID(parentBatchFile.BatchID, cancellationToken);
        if (batch == null)
            return Result.Failure<BatchFile>("Unable to create batch file because the batch ID doesn't exists.");

        if (await batchFileQueries.BatchFileExistsByFilePath(filePath, cancellationToken))
            return Result.Failure<BatchFile>("One batch file already exists with the same file path.");

        var batchFile = BatchFile.CreateExtractedBatchFile(parentBatchFile.ID, batch.ID, filePath);

        var batchFileID = await batchFileRepository.Insert(batchFile, cancellationToken);

        var setIDResult = batchFile.SetID(batchFileID);
        if (setIDResult.IsFailure)
            return Result.Failure<BatchFile>(setIDResult.Error);

        return Result.Success(batchFile);
    }

    public async Task<Result> UpdateBatchFile(BatchFile batchFile, Func<Result> func, CancellationToken cancellationToken)
    {
        try
        {
            //await databaseFactory.BeginAsync();

            var funcResult = func.Invoke();
            if (funcResult.IsFailure)
                return funcResult;

            await batchFileRepository.Update(batchFile, cancellationToken);

            //await databaseFactory.CommitAsync();

            return Result.Success(batchFile);
        }
        catch (Exception ex)
        {
            const string errorMessage = "An exception occured while trying to update the batch.";

            _logger.Error(ex, errorMessage);

            return Result.Failure<Batch>(errorMessage);
        }
    }
}
