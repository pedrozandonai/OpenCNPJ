using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Models.Enums;
using OpenCnpj.Core.Constants;
using System.Diagnostics.CodeAnalysis;

namespace OpenCnpj.Application.Batches.Batches.Domain;
public class Batch
{
    public int ID { get; private set; }
    public string Identifier { get; private set; }
    public EBatchOperation Operation { get; private set; }
    public EOperationStatus OperationStatus { get; private set; }
    public string? OperationFailureDescription { get; private set; }
    public string? Directory { get; private set; }
    public DateTime? RetryDate { get; private set; }

    [ExcludeFromCodeCoverage]
    private Batch()
    {
        // TODO: Por algum motivo, o dapper ta mapeando errado os enums, descobrir pq depois.
    }

    private Batch(int id, string identifier, EBatchOperation operation, EOperationStatus operationStatus, string? operationFailureDescription, string? directory, DateTime? retryDate)
    {
        ID = id;
        Identifier = identifier;
        Operation = operation;
        OperationStatus = operationStatus;
        OperationFailureDescription = operationFailureDescription;
        Directory = directory;
        RetryDate = retryDate;
    }

    public static Batch Create(string identifier)
        => new(0, identifier, EBatchOperation.Created, EOperationStatus.Success, null, null, null);

    public void SetID(int id)
        => ID = id;

    public void Update(EBatchOperation newStatus)
    {
        Operation = newStatus;
    }

    public Result CreateBatchDirectory()
    {
        try
        {
            var directory = Path.Combine(Paths.GovDataFolder, Identifier);

            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            Directory = directory;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(string.Format("An error occurred while trying to create the batch directory for the files. Exception: {0}", ex.ToString()));
        }
    }

    public Result DeleteBatchDirectory()
    {
        try
        {
            var verificationResult = VerifyIfDirectoryExists();
            if (verificationResult.IsFailure)
                return verificationResult;

            if (System.IO.Directory.Exists(Directory))
                return Result.Failure("The application could not find the directory of the batch files.");

            System.IO.Directory.Delete(Directory!);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(string.Format("An error occurred while trying to delete the batch directory. Exception: {0}", ex.ToString()));
        }
    }

    public Result VerifyIfDirectoryExists()
    {
        if (string.IsNullOrEmpty(Directory))
            return Result.Failure("The directory of the batch files doesn't exists.");

        return Result.Success();
    }

    public string GetRawDirectoryByBatch()
        => Path.Combine(Directory!, "raw");

    public string GetExtractedDirectoryByBatch()
        => Path.Combine(Directory!, "extracted");

    public Result<EBatchOperation> GetBatchNextOperation()
    {
        switch (OperationStatus)
        {
            case EOperationStatus.Failure:
                return Result.Success(Operation);
            case EOperationStatus.InOperation:
                return Result.Failure<EBatchOperation>("Cannot get the next operation while the batch is in a operation.");
        }

        EBatchOperation? nextBatchOperation = null;
        switch (Operation)
        {
            case EBatchOperation.Created or EBatchOperation.PendingGovernmentBatch:
                nextBatchOperation = EBatchOperation.RenamingMongoCollections;
                break;

            //case EBatchOperation.DownloadingFiles:
            //    nextBatchOperation = EBatchOperation.ExtractingFiles;
            //    break;

            //case EBatchOperation.ExtractingFiles:
            //    nextBatchOperation = EBatchOperation.ProcessingCSVFiles;
            //    break;

            //case EBatchOperation.ProcessingCSVFiles:
            //    nextBatchOperation = EBatchOperation.RenamingMongoCollections;
            //    break;

            case EBatchOperation.RenamingMongoCollections:
                nextBatchOperation = EBatchOperation.Finished;
                break;
        }

        if (!nextBatchOperation.HasValue)
            return Result.Failure<EBatchOperation>("Unable to resolve batch next operation.");

        return Result.Success(nextBatchOperation.Value);
    }

    public Result StartRenamingMongoCollections()
    {
        var operation = EBatchOperation.RenamingMongoCollections;

        if (!IsInCurrentOperationError(operation) && Operation != EBatchOperation.Created && OperationStatus != EOperationStatus.Success)
            return Result.Failure("Cannot start renaming the mongo collections in the current batch operation and operation status.");

        Operation = operation;
        OperationStatus = EOperationStatus.InOperation;

        return Result.Success();
    }

    public Result SetFinishedBatch()
    {
        var operation = EBatchOperation.Finished;

        if (!IsInCurrentOperationError(operation) && Operation != EBatchOperation.RenamingMongoCollections && OperationStatus != EOperationStatus.Success)
            return Result.Failure("Cannot set the batch to finalized in the current batch operation and operation status.");

        Operation = operation;
        OperationStatus = EOperationStatus.Success;

        return Result.Success();
    }

    public Result SetPendingGovernmentBatch()
    {
        if (Operation != EBatchOperation.Created)
            return Result.Failure("Cannot set pending government batch operation with the batch status different from 'Downloading Files'.");

        Operation = EBatchOperation.PendingGovernmentBatch;
        OperationStatus = EOperationStatus.InOperation;

        if (RetryDate.HasValue)
        {
            RetryDate = RetryDate.Value.AddDays(1);
            return Result.Success();
        }

        var now = DateTime.Now;

        RetryDate = new DateTime(now.Year, now.Month, 1).AddMonths(1);

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
            return Result.Failure("The batch operation status cannot be updated from a status different from 'In Operation'.");

        OperationStatus = newBatchOperationStatus;

        if (newBatchOperationStatus == EOperationStatus.Failure)
        {
            if (string.IsNullOrEmpty(operationFailureReason))
                return Result.Failure("Failure operations status require a reason string");

            OperationFailureDescription = operationFailureReason;
        }

        return Result.Success();
    }

    private bool IsInCurrentOperationError(EBatchOperation operation)
        => Operation == operation && OperationStatus == EOperationStatus.Failure;
}
