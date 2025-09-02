using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Services;
public interface IAddressTypeService
{
    Task<Result<AddressType>> GetOrCreateAddressTypeByStreetType(string streetType, CancellationToken cancellationToken);
}