using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using Serilog;

namespace OpenCnpj.Application.Batches.BatchFiles.Services;
//public class BatchFileService(IBatchRepository batchQueries, IBatchFileRepository batchFileRepository, ILogger logger) : IBatchFileService
//{
//    private readonly ILogger _logger = logger.ForContext<BatchFileService>();

//    public async Task<Result> CreateNewBatchFile(int batchId, string fileName, string filePath, CancellationToken cancellationToken)
//    {
//        try
//        {
//            if (await batchQueries.BatchExistsById(batchId, cancellationToken) == null)
//                return Result.Failure("Unable to create a Batch File because the Batch Id doesn't exists.");

//            var batchFile = await batchFileRepository.Get(batchId, fileName, cancellationToken);
//            if (batchFile != null)
//                return Result.Failure("There is a file with the same batch id and name already created.");

//            batchFile = BatchFile.Create(batchId, fileName, filePath);

//            await batchFileRepository.Insert(batchFile, cancellationToken);

//            return Result.Success();
//        }
//        catch (Exception ex)
//        {
//            _logger.Error(ex, "An exception occurred while trying to create a new batch file.");

//            return Result.Failure("An error occurred while trying to create a new batch file.");
//        }
//    }
//}
