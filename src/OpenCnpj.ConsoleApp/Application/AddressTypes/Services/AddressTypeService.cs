using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Repositories;
using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Services;

public class AddressTypeService(IAddressTypeRepository addressTypeRepository) : IAddressTypeService
{
    public async Task<Result<AddressType>> GetOrCreateAddressTypeByStreetType(string streetType, CancellationToken cancellationToken)
    {
        var addressType = await addressTypeRepository.GetByDescription(streetType, cancellationToken);

        if (addressType == null)
        {
            addressType = AddressType.Create(0, streetType);

            var addressTypeID = await addressTypeRepository.Insert(addressType, cancellationToken);

            var setIdResult = addressType.SetID(addressTypeID);
            if (setIdResult.IsFailure)
                return Result.Failure<AddressType>(setIdResult.Error);
        }

        return Result.Success(addressType);
    }
}
