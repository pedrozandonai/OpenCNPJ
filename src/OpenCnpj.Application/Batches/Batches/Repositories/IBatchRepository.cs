using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.Batches.Repositories;
public interface IBatchRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(Batch batch, CancellationToken cancellationToken);
    Task Update(Batch batch, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken);
}