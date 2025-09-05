using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class AddressRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IAddressRepository
{
    public async Task Insert(IEnumerable<Address> addresses, CancellationToken cancellationToken)
    {
        openCnpjDbContext.AttachRange(addresses.Select(a => a.City));
        
        await openCnpjDbContext.Addresses.AddRangeAsync(addresses, cancellationToken);
    }

    public async Task Insert(Address address, CancellationToken cancellationToken)
    {
        openCnpjDbContext.Attach(address.City);

        await openCnpjDbContext.Addresses.AddAsync(address, cancellationToken);
    }
}
