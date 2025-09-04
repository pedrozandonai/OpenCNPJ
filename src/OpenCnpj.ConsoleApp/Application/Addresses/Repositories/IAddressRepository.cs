using OpenCnpj.ConsoleApp.Application.Addresses.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Addresses.Repositories;
public interface IAddressRepository : IOpenCnpjDatabaseFactory
{
    Task<long> Insert(Address address, CancellationToken cancellationToken);
    Task Insert(IEnumerable<Address> addressess, CancellationToken cancellationToken);
    Task CopyToTable(IEnumerable<Address> addresses, CancellationToken cancellationToken);
}