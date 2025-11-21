namespace OpenCnpj.Application.Batches.Batches.Queries;

public interface IBatchQueries
{
    Task<bool> BatchExistsByID(int id, CancellationToken cancellationToken);
}