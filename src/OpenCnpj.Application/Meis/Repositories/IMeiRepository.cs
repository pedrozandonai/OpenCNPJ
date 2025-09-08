using OpenCnpj.Application.Meis.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Meis.Repositories;
public interface IMeiRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<Mei> meis, CancellationToken cancellationToken);
}