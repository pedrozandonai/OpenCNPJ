using CSharpFunctionalExtensions;
using OpenCnpj.Application.ApplicationSteps.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Repositories;
using Serilog;

namespace OpenCnpj.Application.Batches.Batches.Services;

public class BatchService(IBatchRepository batchRepository, ILogger logger) : IBatchService
{
    private readonly ILogger _logger = logger.ForContext<BatchService>();

    public async Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            await batchRepository.DatabaseFactory.BeginAsync();

            var batch = Batch.Create(identifier);

            var batchDirectoryCreationResult = batch.CreateBatchDirectory();
            if (batchDirectoryCreationResult.IsFailure)
                return Result.Failure<Batch>(batchDirectoryCreationResult.Error);

            var batchID = await batchRepository.Insert(batch, cancellationToken);

            batch.SetID(batchID);

            await batchRepository.DatabaseFactory.CommitAsync();

            _logger.Information("Created batch {0} for period {1}.", batch.ID, batch.Identifier);

            return Result.Success(batch);
        }
        catch (Exception ex)
        {
            const string errorMessage = "An exception occured while trying to create a new batch.";

            _logger.Error(ex, errorMessage);

            return Result.Failure<Batch>(errorMessage);
        }
    }
    
    public async Task<Result<Batch>> UpdateBatchStatus(Batch batch, string newStatus, CancellationToken cancellationToken)
    {
        try
        {
            await batchRepository.DatabaseFactory.BeginAsync();

            batch.Update(newStatus);

            await batchRepository.Update(batch, cancellationToken);

            await batchRepository.DatabaseFactory.CommitAsync();

            return Result.Success(batch);
        }
        catch (Exception ex)
        {
            const string errorMessage = "An exception occured while trying to update the batch.";

            _logger.Error(ex, errorMessage);

            return Result.Failure<Batch>(errorMessage);
        }
    }

    public async Task<Result> SetApplicationLastStep(Batch batch, EApplicationStep lastApplicationStep, CancellationToken cancellationToken)
    {
        await batchRepository.DatabaseFactory.BeginAsync();

        bool isLastStepInvalid = false;
        
        switch (batch.ApplicationLastStepID)
        {
            case EApplicationStep.StartedApplication:
                if (lastApplicationStep != EApplicationStep.DownloadingFiles)
                    isLastStepInvalid = true;
                break;
            
            case EApplicationStep.DownloadingFiles:
                if (lastApplicationStep != EApplicationStep.ExtractingFiles)
                    isLastStepInvalid = true;
                break;
            
            case EApplicationStep.ExtractingFiles:
                if (lastApplicationStep != EApplicationStep.FormattingRawData)
                    isLastStepInvalid = true;
                break;
            
            default:
                return Result.Failure("Could not recognize the last application step.");
        }
        
        if (isLastStepInvalid)
            return Result.Failure("The last step is invalid.");
        
        batch.SetLastStep(lastApplicationStep);
        
        await batchRepository.DatabaseFactory.CommitAsync();

        return Result.Success();
    }
}
