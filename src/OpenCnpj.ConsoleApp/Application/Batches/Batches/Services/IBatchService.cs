using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
public interface IBatchService
{
    Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken);
    Task<Result<Batch>> UpdateBatchStatus(Batch batch, string newStatus, CancellationToken cancellationToken);

    Task<Result> SetApplicationLastStep(Batch batch, EApplicationStep lastApplicationStep, CancellationToken cancellationToken);
}