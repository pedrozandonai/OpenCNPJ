using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Addresses.Domain;
public interface IAddressRepository : IBaseRepository
{
    Task Insert(IEnumerable<Address> addresses, CancellationToken cancellationToken);
    Task Insert(Address address, CancellationToken cancellationToken);
}
