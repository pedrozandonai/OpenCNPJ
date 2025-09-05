using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class AddressTypeRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IAddressTypeRepository
{
    public async Task Insert(IEnumerable<AddressType> addressTypes, CancellationToken cancellationToken)
    {
        openCnpjDbContext.AttachRange(addressTypes);

        await openCnpjDbContext.AddressTypes.AddRangeAsync(addressTypes, cancellationToken);
    }

    public async Task Insert(AddressType addressType, CancellationToken cancellationToken)
        => await openCnpjDbContext.AddressTypes.AddAsync(addressType, cancellationToken);

    public async Task<IEnumerable<AddressType>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.AddressTypes
        .ToListAsync(cancellationToken);

    public async Task<AddressType?> GetByDescription(string description, CancellationToken cancellationToken)
        => await openCnpjDbContext.AddressTypes
        .Where(a => a.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefaultAsync(cancellationToken);
}
