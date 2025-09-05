using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class BatchRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IBatchRepository
{
    public async Task<Batch> Insert(Batch batch, CancellationToken cancellationToken)
    {
        await openCnpjDbContext.AddAsync(batch, cancellationToken);
        return batch;
    }

    public async Task<Batch?> BatchExistsById(int batchId, CancellationToken cancellationToken)
        => await openCnpjDbContext.Batches
        .Where(b => b.Id == batchId)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken)
        => await openCnpjDbContext.Batches
        .Where(b => b.Identifier.Equals(identifier))
        .FirstOrDefaultAsync(cancellationToken);
}
