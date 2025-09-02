using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Models.Enums;

namespace OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Domain;

public class BatchFile
{
    public int ID { get; init; }
    public int BatchID { get; private set; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public EFileStatus FileStatusID { get; private set; }

    private BatchFile(int iD, int batchID, string fileName, string filePath, EFileStatus fileStatusID)
    {
        ID = iD;
        BatchID = batchID;
        FileName = fileName;
        FilePath = filePath;
        FileStatusID = fileStatusID;
    }

    public static BatchFile Create(int batchID, string fileName, string filePath)
        => new(0, batchID, fileName, filePath, EFileStatus.Downloading);

    public Result UpdateFileStatus(EFileStatus newFileStatusID)
    {
        bool isNewFileStatusInvalid = false;

        switch(FileStatusID)
        {
            case EFileStatus.Downloading:
                if (newFileStatusID != EFileStatus.Created)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Created:
                if (newFileStatusID != EFileStatus.Processing)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Processing:
                if (newFileStatusID != EFileStatus.Processed)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Processed:
                if (newFileStatusID != EFileStatus.Deleted)
                    isNewFileStatusInvalid = true;
                break;
        }

        if (isNewFileStatusInvalid)
            return Result.Failure("The new file status is invalid.");

        FileStatusID = newFileStatusID;

        return Result.Success();
    }
}
