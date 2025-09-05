using CSharpFunctionalExtensions;
using OpenCnpj.Application.AddressTypes.Domain;

namespace OpenCnpj.Application.AddressTypes.Services;
public interface IAddressTypeService
{
    Task<Result<AddressType>> GetOrCreateAddressTypeByStreetType(string streetType, CancellationToken cancellationToken);
}