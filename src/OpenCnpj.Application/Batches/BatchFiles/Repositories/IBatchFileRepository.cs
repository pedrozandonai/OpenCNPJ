using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Application.Batches.BatchFiles.Repositories;
public interface IBatchFileRepository
{
    Task Insert(BatchFile batchFile, CancellationToken cancellationToken);
    Task Update(BatchFile batchFile, CancellationToken cancellationToken);
    Task<BatchFile?> Get(int batchID, string fileName, CancellationToken cancellationToken);
}