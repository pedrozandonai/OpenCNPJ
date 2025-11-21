using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.Application.Batches.Batches.Services;

public class BatchFileService(IDatabaseFactory databaseFactory, IBatchFileRepository batchFileRepository, ILogger logger) : IBatchFileService
{
    private readonly ILogger _logger = logger.ForContext<BatchFileService>();

    public async Task<Result> UpdateBatchFile(BatchFile batchFile, Func<Result> func, CancellationToken cancellationToken)
    {
        try
        {
            await databaseFactory.BeginAsync();

            var funcResult = func.Invoke();
            if (funcResult.IsFailure)
                return funcResult;

            await batchFileRepository.Update(batchFile, cancellationToken);

            await databaseFactory.CommitAsync();

            return Result.Success(batchFile);
        }
        catch (Exception ex)
        {
            const string errorMessage = "An exception occured while trying to update the batch.";

            _logger.Error(ex, errorMessage);

            return Result.Failure<Batch>(errorMessage);
        }
    }
}
