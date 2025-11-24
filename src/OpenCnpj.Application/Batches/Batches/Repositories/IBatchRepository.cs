using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Batches.Batches.Repositories;
public interface IBatchRepository
{
    Task<int> Insert(Batch batch, CancellationToken cancellationToken);
    Task Update(Batch batch, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByPeriod(string period, CancellationToken cancellationToken);
    Task<Batch?> GetByID(int id, CancellationToken cancellationToken);
}