using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Repositories;
public interface IBatchRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(Batch batch, CancellationToken cancellationToken);
    Task Update(Batch batch, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken);
}