using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Batches.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken);
    Task<Result> UpdateBatch(Batch batch, Func<Result> func, CancellationToken cancellationToken);
}