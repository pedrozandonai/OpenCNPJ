using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.BatchFiles.Models.Dtos;
using OpenCnpj.Application.Batches.Models.Enums;

namespace OpenCnpj.Application.Batches.BatchFiles.Domain;
public class BatchFile
{
    public int ID { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public EFileOperation FileOperation { get; private set; }
    public EOperationStatus OperationStatus { get; private set; }

    public BatchFile(string fileName, string filePath, EFileOperation fileOperation, EOperationStatus operationStatus)
    {
        FileName = fileName;
        FilePath = filePath;
        FileOperation = fileOperation;
        OperationStatus = operationStatus;
    }

    public static BatchFile Create(string fileName, string filePath)
        => new(fileName, filePath, EFileOperation.Downloading, EOperationStatus.InOperation);

    public Result SetID(int id)
    {
        if (ID != default)
            return Result.Failure("The ID for the batch file already has been defined.");

        ID = id;

        return Result.Success();
    }

    public Result SetDownloadCompleted(string finalPath)
    {
        if (FileOperation != EFileOperation.Downloading && OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("Cannot finish the file download state.");

        if (!Path.Exists(finalPath))
            return Result.Failure("The final path doesn't exists.");

        OperationStatus = EOperationStatus.Success;
        FileName = Path.GetFileName(finalPath);
        FilePath = finalPath;

        return Result.Success();
    }

    public Result StartExtractingFile()
    {
        if (FileOperation != EFileOperation.Downloading || OperationStatus != EOperationStatus.Success)
            return Result.Failure("Cannot start extracting file while in download state.");

        FileOperation = EFileOperation.Extracting;
        OperationStatus = EOperationStatus.InOperation;

        return Result.Success();
    }

    public Result SetExtranctionCompleted(string finalPath)
    {
        if (FileOperation != EFileOperation.Extracting && OperationStatus != EOperationStatus.InOperation)
            return Result.Failure("Cannot finish the file download state.");

        if (!Path.Exists(finalPath))
            return Result.Failure("The final path doesn't exists.");

        OperationStatus = EOperationStatus.Success;
        FileName = Path.GetFileName(finalPath);
        FilePath = finalPath;

        return Result.Success();
    }
}
