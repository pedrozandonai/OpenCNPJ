using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Domain;

namespace OpenCnpj.Application.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> GetOrCreateBatchByIdentifier(string batchIdentifier, CancellationToken cancellationToken);
    Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken);
    Task<Result> UpdateBatch(Batch batch, Func<Result> func, CancellationToken cancellationToken);
}