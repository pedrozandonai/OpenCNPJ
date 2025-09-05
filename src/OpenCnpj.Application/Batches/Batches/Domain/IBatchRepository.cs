using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Batches.Batches.Domain;

public interface IBatchRepository : IBaseRepository
{
    Task<Batch> Insert(Batch batch, CancellationToken cancellationToken);
    Task<Batch?> BatchExistsById(int batchId, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken);
}
