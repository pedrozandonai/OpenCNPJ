using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Batches.BatchFiles.Services;
public interface IBatchFileService
{
    Task<Result> CreateNewBatchFile(int batchId, string fileName, string filePath, CancellationToken cancellationToken);
}