using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Application.Batches.Batches.Services;
public interface IBatchFileService
{
    Task<Result<Batch>> GetOrCreateBatchByIdentifier(string batchIdentifier, CancellationToken cancellationToken);
    Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken);
    Task<Result> UpdateBatchFile(BatchFile batchFile, Func<Result> func, CancellationToken cancellationToken);
}