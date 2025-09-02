using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Services;
public interface IBatchFileService
{
    Task<Result> CreateNewBatchFile(int batchID, string fileName, string filePath, CancellationToken cancellationToken);
}