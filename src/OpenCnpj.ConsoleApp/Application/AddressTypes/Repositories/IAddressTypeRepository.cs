using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Repositories;

public interface IAddressTypeRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(AddressType addressType, CancellationToken cancellationToken);
    Task Insert(IEnumerable<AddressType> addressTypes, CancellationToken cancellationToken);
    Task<AddressType?> GetByDescription(string description, CancellationToken cancellationToken);
    Task<IEnumerable<AddressType>> GetAll(CancellationToken cancellationToken);
}