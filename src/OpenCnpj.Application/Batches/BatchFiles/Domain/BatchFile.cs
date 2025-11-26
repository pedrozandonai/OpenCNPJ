using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.BatchFiles.Models.Dtos;
using OpenCnpj.Application.Batches.BatchFiles.Models.Enums;
using OpenCnpj.Application.Batches.Models.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OpenCnpj.Application.Batches.BatchFiles.Domain;
public class BatchFile
{
    public int ID { get; private set; }
    public int? ParentBatchFileID { get; init; }
    public int BatchID { get; init; }
    public string? Url { get; init; }
    public string Extension { get; init; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public EBatchFileOperation FileOperation { get; private set; }
    public EOperationStatus OperationStatus { get; private set; }
    public EBatchFileType Type { get; init; }
    public string? OperationFailureDescription { get; private set; }
    public bool IsFileDeleted { get; private set; }
    public DateTime CreatedAt { get; init; }

    private BatchFile(int? parentBatchFileID, int batchID, string? url, string extension, string fileName, string filePath, EBatchFileOperation fileOperation, EOperationStatus operationStatus, EBatchFileType type, string? operationFailureDescription, bool isFileDeleted, DateTime createdAt)
    {
        ParentBatchFileID = parentBatchFileID;
        BatchID = batchID;
        Url = url;
        Extension = extension;
        FileName = fileName;
        FilePath = filePath;
        FileOperation = fileOperation;
        OperationStatus = operationStatus;
        Type = type;
        OperationFailureDescription = operationFailureDescription;
        IsFileDeleted = isFileDeleted;
        CreatedAt = createdAt;
    }

    [ExcludeFromCodeCoverage]
    private BatchFile()
    {
        // TODO: Por algum motivo, o dapper ta mapeando errado os enums, descobrir pq depois.
    }

    public static BatchFile CreatePartialDownloadBatchFile(int batchID, string url, string filePath)
        => new(null, batchID, url, "partial", Path.GetFileName(filePath), filePath, EBatchFileOperation.Downloading, EOperationStatus.InOperation, EBatchFileType.PartialDownloadedFile, null, false, DateTime.Now);

    public static BatchFile CreateDownloadedBatchFile(int parentBatchFileID, int batchID,  string filePath)
        => new(parentBatchFileID, batchID, null, Path.GetExtension(filePath).Replace(".", ""), Path.GetFileNameWithoutExtension(filePath), filePath, EBatchFileOperation.Downloading, EOperationStatus.Created, EBatchFileType.DownloadedFile, null, false, DateTime.Now);

    public static BatchFile CreateExtractedBatchFile(int parentBatchFileID, int batchID, string filePath)
        => new(parentBatchFileID, batchID, null, Path.GetExtension(filePath).Replace(".", ""), Path.GetFileNameWithoutExtension(filePath), filePath, EBatchFileOperation.Extracting, EOperationStatus.Created, EBatchFileType.ExtractedFile, null, false, DateTime.Now);

    public Result SetID(int id)
    {
        if (ID != default)
            return Result.Failure("The ID for the batch file already has been defined.");

        ID = id;

        return Result.Success();
    }

    public Result SetDownloadCompleted()
    {
        if (Type != EBatchFileType.PartialDownloadedFile)
            return Result.Failure("Cannot set a non partial downloaded file to completed.");

        if (FileOperation != EBatchFileOperation.Downloading && OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("Cannot finish the file download state.");

        IsFileDeleted = true;

        var setOperationSuccessResult = SetOperationSuccess();
        if (setOperationSuccessResult.IsFailure)
            return setOperationSuccessResult;

        return Result.Success();
    }

    public Result StartExtractingFile()
    {
        if (Type != EBatchFileType.DownloadedFile)
            return Result.Failure("Cannot start extracting a non downloaded file.");

        var operation = EBatchFileOperation.Extracting;

        if (!IsInCurrentOperationError(operation) && FileOperation != EBatchFileOperation.Downloading && OperationStatus != EOperationStatus.Created)
            return Result.Failure("Cannot start extracting file while in download state.");

        FileOperation = operation;
        OperationStatus = EOperationStatus.InOperation;

        return Result.Success();
    }

    public Result SetExtranctionCompleted()
    {
        if (Type != EBatchFileType.DownloadedFile)
            return Result.Failure("Cannot set the extraction completed in a non downlodable file.");

        if (FileOperation != EBatchFileOperation.Extracting && OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("Cannot finish the file download state.");

        var deleteFileResult = DeleteFile();
        if (deleteFileResult.IsFailure)
            return deleteFileResult;

        var setOperationSuccessResult = SetOperationSuccess();
        if (setOperationSuccessResult.IsFailure)
            return setOperationSuccessResult;

        return Result.Success();
    }

    public Result StartProcessingFile()
    {
        if (Type != EBatchFileType.ExtractedFile)
            return Result.Failure("Cannot start processing a non extracted file.");

        var operation = EBatchFileOperation.Processing;

        if (!IsInCurrentOperationError(operation) && FileOperation != EBatchFileOperation.Extracting && OperationStatus != EOperationStatus.Created)
            return Result.Failure("Cannot start processing the file while in extracting state.");

        FileOperation = operation;
        OperationStatus = EOperationStatus.InOperation;

        return Result.Success();
    }

    public Result SetProcessingCompleted()
    {
        if (Type != EBatchFileType.ExtractedFile)
            return Result.Failure("Cannot set the pprocessing completed in a non extracted file.");

        if (FileOperation != EBatchFileOperation.Processing && OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("Cannot finish the file processing state.");

        FileOperation = EBatchFileOperation.Finished;

        var deleteFileResult = DeleteFile();
        if (deleteFileResult.IsFailure)
            return deleteFileResult;

            var setOperationSuccessResult = SetOperationSuccess();
        if (setOperationSuccessResult.IsFailure)
            return setOperationSuccessResult;

        return Result.Success();
    }

    public Result SetOperationFailure(string operationFailureReason)
        => SwithOperationStatus(EOperationStatus.Failure, operationFailureReason);

    public Result SetOperationSuccess()
    {
        if (!string.IsNullOrEmpty(OperationFailureDescription))
            OperationFailureDescription = null;

        return SwithOperationStatus(EOperationStatus.Success);
    }

    private Result SwithOperationStatus(EOperationStatus newBatchOperationStatus, string? operationFailureReason = "")
    {
        if (OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("The batch file operation status cannot be updated from a status different from 'In Operation'.");

        OperationStatus = newBatchOperationStatus;

        if (newBatchOperationStatus == EOperationStatus.Failure)
        {
            if (string.IsNullOrEmpty(operationFailureReason))
                return Result.Failure("Failure operations status require a reason string");

            OperationFailureDescription = operationFailureReason;
        }

        return Result.Success();
    }

    private Result DeleteFile()
    {
        if (IsFileDeleted)
            return Result.Failure("Cannot delete a file that is already deleted.");

        try
        {
            File.Delete(FilePath);

            IsFileDeleted = true;
        }
        catch(Exception ex)
        {
            return Result.Failure(string.Format("An error occurred while trying to delete the file. Error: {0}", ex.Message));
        }

        return Result.Success();
    }

    private bool IsInCurrentOperationError(EBatchFileOperation operation)
        => FileOperation == operation && OperationStatus == EOperationStatus.Failure;
}
