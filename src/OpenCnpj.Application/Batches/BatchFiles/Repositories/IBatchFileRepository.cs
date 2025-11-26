using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Application.Batches.BatchFiles.Repositories;
public interface IBatchFileRepository
{
    Task<int> Insert(BatchFile batchFile, CancellationToken cancellationToken);
    Task Update(BatchFile batchFile, CancellationToken cancellationToken);
    Task<IEnumerable<BatchFile>> GetUnfinishedBatchFileOperationsByBatchID(int batchID, CancellationToken cancellationToken);
}