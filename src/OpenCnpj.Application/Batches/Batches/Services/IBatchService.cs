using CSharpFunctionalExtensions;
using OpenCnpj.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Batches.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken);
    Task<Result<Batch>> UpdateBatchStatus(Batch batch, string newStatus, CancellationToken cancellationToken);

    Task<Result> SetApplicationLastStep(Batch batch, EApplicationStep lastApplicationStep, CancellationToken cancellationToken);
}