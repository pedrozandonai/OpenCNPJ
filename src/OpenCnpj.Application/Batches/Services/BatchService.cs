using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Domain;
using OpenCnpj.Application.Batches.Repositories;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.Application.Batches.Services;

public class BatchService(IDatabaseFactory databaseFactory, IBatchRepository batchRepository, ILogger logger) : IBatchService
{
    private readonly ILogger _logger = logger.ForContext<BatchService>();

    public async Task<Result<Batch>> GetOrCreateBatchByIdentifier(string batchIdentifier, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetBatchByIdentifier(batchIdentifier, cancellationToken);
        if (batch == null)
        {
            var batchCreationResult = await CreateNewBatch(batchIdentifier, cancellationToken);
            if (batchCreationResult.IsFailure)
                return batchCreationResult;

            batch = batchCreationResult.Value;
        }

        return Result.Success(batch);
    }

    public async Task<Result<Batch>> CreateNewBatch(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            await databaseFactory.BeginAsync();

            var batch = Batch.Create(identifier);

            var batchDirectoryCreationResult = batch.CreateBatchDirectory();
            if (batchDirectoryCreationResult.IsFailure)
                return Result.Failure<Batch>(batchDirectoryCreationResult.Error);

            var batchID = await batchRepository.Insert(batch, cancellationToken);

            batch.SetID(batchID);

            await databaseFactory.CommitAsync();

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

    public async Task<Result> UpdateBatch(Batch batch, Func<Result> func, CancellationToken cancellationToken)
    {
        try
        {
            await databaseFactory.BeginAsync();

            var funcResult = func.Invoke();
            if (funcResult.IsFailure)
                return funcResult;

            await batchRepository.Update(batch, cancellationToken);

            await databaseFactory.CommitAsync();

            return Result.Success(batch);
        }
        catch (Exception ex)
        {
            const string errorMessage = "An exception occured while trying to update the batch.";

            _logger.Error(ex, errorMessage);

            return Result.Failure<Batch>(errorMessage);
        }
    }
}
