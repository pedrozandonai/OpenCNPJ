using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Batches.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> GetOrCreateBatchByPeriod(string batchPeriod, CancellationToken cancellationToken);
    Task<Result<Batch>> CreateFutureBatch(CancellationToken cancellationToken);
    Task<Result<Batch>> CreateNewBatch(string period, CancellationToken cancellationToken);
    Task<Result> UpdateBatch(Batch batch, Func<Result> func, CancellationToken cancellationToken);
}