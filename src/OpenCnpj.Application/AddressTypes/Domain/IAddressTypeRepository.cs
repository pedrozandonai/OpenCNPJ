using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.AddressTypes.Domain;
public interface IAddressTypeRepository : IBaseRepository
{
    Task Insert(IEnumerable<AddressType> addressTypes, CancellationToken cancellationToken);
    Task Insert(AddressType addressType, CancellationToken cancellationToken);
    Task<IEnumerable<AddressType>> GetAll(CancellationToken cancellationToken);
    Task<AddressType?> GetByDescription(string description, CancellationToken cancellationToken);
}
