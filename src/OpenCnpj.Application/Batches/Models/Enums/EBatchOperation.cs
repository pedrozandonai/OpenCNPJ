namespace OpenCnpj.Application.Batches.Models.Enums;
public enum EBatchOperation
{
    Created = 1,
    PendingGovernmentBatch,
    DownloadingFiles,
    ExtractingFiles,
    ProcessingCSVFiles,
    RenamingMongoCollections,
    Finished
}
