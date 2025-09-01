using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> CreateNewBatch(CancellationToken cancellationToken);
    Task<Result<Batch>> UpdateBatchStatus(Batch batch, string newStatus, CancellationToken cancellationToken);
}