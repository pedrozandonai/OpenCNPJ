using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Queries;
using OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Domain;
using OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Repositories;
using Serilog;

namespace OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Services;
public class BatchFileService(IBatchQueries batchQueries, IBatchFileRepository batchFileRepository, ILogger logger) : IBatchFileService
{
    private readonly ILogger _logger = logger.ForContext<BatchFileService>();

    public async Task<Result> CreateNewBatchFile(int batchID, string fileName, string filePath, CancellationToken cancellationToken)
    {
        try
        {
            if (!await batchQueries.BatchExistsByID(batchID, cancellationToken))
                return Result.Failure("Unable to create a Batch File because the Batch ID doesn't exists.");

            var batchFile = await batchFileRepository.Get(batchID, fileName, cancellationToken);
            if (batchFile != null)
                return Result.Failure("There is a file with the same batch id and name already created.");

            batchFile = BatchFile.Create(batchID, fileName, filePath);

            await batchFileRepository.Insert(batchFile, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occurred while trying to create a new batch file.");

            return Result.Failure("An error occurred while trying to create a new batch file.");
        }
    }
}
