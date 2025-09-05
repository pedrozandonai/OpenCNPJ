using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Addresses.Repositories;
public interface IAddressRepository : IOpenCnpjDatabaseFactory
{
    Task<long> Insert(Address address, CancellationToken cancellationToken);
    Task Insert(IEnumerable<Address> addressess, CancellationToken cancellationToken);
    Task CopyToTable(IEnumerable<Address> addresses, CancellationToken cancellationToken);
}