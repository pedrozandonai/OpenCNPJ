using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Repositories;
using Serilog;

namespace OpenCnpj.Application.Batches.Batches.Services;

public class BatchService(IBatchRepository batchRepository, ILogger logger) : IBatchService
{
    private readonly ILogger _logger = logger.ForContext<BatchFileService>();

    public async Task<Result<Batch>> GetOrCreateBatchByPeriod(string batchPeriod, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetBatchByPeriod(batchPeriod, cancellationToken);
        if (batch == null)
        {
            var batchCreationResult = await CreateNewBatch(batchPeriod, cancellationToken);
            if (batchCreationResult.IsFailure)
                return batchCreationResult;

            batch = batchCreationResult.Value;
        }

        return Result.Success(batch);
    }

    public async Task<Result<Batch>> CreateFutureBatch(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var futureBatchPeriod = new DateTime(now.Year, now.Month, 1).AddMonths(1).ToString();

        var batch = await GetOrCreateBatchByPeriod(futureBatchPeriod, cancellationToken);
        if (batch.IsFailure)
            return batch;

        return Result.Success(batch.Value);
    }

    public async Task<Result<Batch>> CreateNewBatch(string period, CancellationToken cancellationToken)
    {
        try
        {
            var batch = Batch.Create(period);

            var batchDirectoryCreationResult = batch.CreateBatchDirectory();
            if (batchDirectoryCreationResult.IsFailure)
                return Result.Failure<Batch>(batchDirectoryCreationResult.Error);

            var batchID = await batchRepository.Insert(batch, cancellationToken);

            batch.SetID(batchID);

            _logger.Information("Created batch {0} for period {1}.", batch.ID, batch.Period);

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
            //await databaseFactory.BeginAsync();

            var funcResult = func.Invoke();
            if (funcResult.IsFailure)
                return funcResult;

            await batchRepository.Update(batch, cancellationToken);

            //await databaseFactory.CommitAsync();

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
