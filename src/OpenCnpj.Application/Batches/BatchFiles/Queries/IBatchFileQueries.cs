
namespace OpenCnpj.Application.Batches.BatchFiles.Queries;

public interface IBatchFileQueries
{
    Task<bool> BatchFileExistsByFilePath(string filePath, CancellationToken cancellationToken);
}