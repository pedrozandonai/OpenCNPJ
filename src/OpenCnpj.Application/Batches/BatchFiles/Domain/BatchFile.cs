using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.BatchFiles.Models.Enums;

namespace OpenCnpj.Application.Batches.BatchFiles.Domain;

public class BatchFile
{
    public int Id { get; init; }
    public int BatchId { get; private set; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public EFileStatus FileStatus { get; private set; }

    private BatchFile(int iD, int batchId, string fileName, string filePath, EFileStatus fileStatus)
    {
        Id = iD;
        BatchId = batchId;
        FileName = fileName;
        FilePath = filePath;
        FileStatus = fileStatus;
    }

    private BatchFile()
    {
    }

    public static BatchFile Create(int batchId, string fileName, string filePath)
        => new(0, batchId, fileName, filePath, EFileStatus.Downloading);

    public Result UpdateFileStatus(EFileStatus newFileStatusId)
    {
        bool isNewFileStatusInvalid = false;

        switch(FileStatus)
        {
            case EFileStatus.Downloading:
                if (newFileStatusId != EFileStatus.Created)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Created:
                if (newFileStatusId != EFileStatus.Processing)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Processing:
                if (newFileStatusId != EFileStatus.Processed)
                    isNewFileStatusInvalid = true;
                break;

            case EFileStatus.Processed:
                if (newFileStatusId != EFileStatus.Deleted)
                    isNewFileStatusInvalid = true;
                break;
        }

        if (isNewFileStatusInvalid)
            return Result.Failure("The new file status is invalid.");

        FileStatus = newFileStatusId;

        return Result.Success();
    }
}
