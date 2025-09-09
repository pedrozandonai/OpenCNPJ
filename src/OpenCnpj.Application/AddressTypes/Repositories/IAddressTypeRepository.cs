using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.AddressTypes.Repositories;

public interface IAddressTypeRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<AddressType> addressTypes, CancellationToken cancellationToken);
    Task<int> Insert(AddressType addressType, CancellationToken cancellationToken);
    Task Insert(IEnumerable<AddressType> addressTypes, CancellationToken cancellationToken);
    Task<AddressType?> GetByDescription(string description, CancellationToken cancellationToken);
    Task<IEnumerable<AddressType>> GetAll(CancellationToken cancellationToken);
}