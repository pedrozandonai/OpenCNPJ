using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.BatchFiles.Repositories;
public interface IBatchFileRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(BatchFile batchFile, CancellationToken cancellationToken);
    Task Update(BatchFile batchFile, CancellationToken cancellationToken);
    Task<BatchFile?> Get(int batchID, string fileName, CancellationToken cancellationToken);
}