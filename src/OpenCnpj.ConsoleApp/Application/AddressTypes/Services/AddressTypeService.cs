using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Repositories;
using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Services;

public class AddressTypeService(IAddressTypeRepository addressTypeRepository) : IAddressTypeService
{
    public async Task<Result<AddressType>> GetOrCreateAddressTypeByStreetType(string streetType, CancellationToken cancellationToken)
    {
        if (!addressTypeRepository.DatabaseFactory.TransactionIsOpen)
            return Result.Failure<AddressType>(ApplicationErrors.NotInTransaction);

        var addressType = await addressTypeRepository.GetByDescription(streetType, cancellationToken);

        if (addressType == null)
        {
            addressType = AddressType.Create(streetType);

            var addressTypeID = await addressTypeRepository.Insert(addressType, cancellationToken);

            var setIdResult = addressType.SetID(addressTypeID);
            if (setIdResult.IsFailure)
                return Result.Failure<AddressType>(setIdResult.Error);
        }

        return Result.Success(addressType);
    }
}
