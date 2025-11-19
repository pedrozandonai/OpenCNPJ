using OpenCnpj.Application.Batches.Domain;

namespace OpenCnpj.Application.Batches.Repositories;
public interface IBatchRepository
{
    Task<int> Insert(Batch batch, CancellationToken cancellationToken);
    Task Update(Batch batch, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken);
}