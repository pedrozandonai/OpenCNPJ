using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Application.Batches.BatchFiles.Repositories;
public class BatchFileRepository : IBatchFileRepository
{
    public async Task<int> Insert(BatchFile batchFile, CancellationToken cancellationToken)
    {
        return default;
    }

    public async Task Update(BatchFile batchFile, CancellationToken cancellationToken)
    {
    }
}
