namespace OpenCnpj.Application.Batches.Batches.Models.Enums;
public enum EBatchOperation
{
    Created = 1,
    PendingGovernmentBatch,
    StartGovernmentPipeline,
    RenamingMongoCollections,
    Finished
}
