using OpenCnpj.Application.Simples.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Simples.Repositories;
public interface ISimpleRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<Simple> simples, CancellationToken cancellationToken);
}