using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Addresses.Domain;
using OpenCnpj.ConsoleApp.Application.Addresses.Repositories;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Services;
using OpenCnpj.ConsoleApp.Application.Cities.Repositories;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Application.Addresses.Services;
public class AddressService(IAddressTypeService addressTypeService, ICityRepository cityRepository, IAddressRepository addressRepository) : IAddressService
{
    public async Task<Result<Address>> CreateAddressByEstablishmentRawRecord(EstablishmentRawRecord establishmentRawRecord, CancellationToken cancellationToken)
    {
        if (!addressRepository.DatabaseFactory.TransactionIsOpen)
            return Result.Failure<Address>(ApplicationErrors.NotInTransaction);

        var establishmentAddress = establishmentRawRecord.Address;

        var addressType = await addressTypeService.GetOrCreateAddressTypeByStreetType(establishmentAddress.StreetType, cancellationToken);
        if (addressType.IsFailure)
            return Result.Failure<Address>(addressType.Error);

        var city = await cityRepository.GetByCode(establishmentAddress.MunicipalityCode, cancellationToken);
        if (city == null)
            return Result.Failure<Address>("The city of the establishment could not be retreived.");

        int? zipCode = null;
        if (int.TryParse(establishmentAddress.ZipCode, out var parsedZipCode))
            zipCode = parsedZipCode;

        int? addressNumber = null;
        if (int.TryParse(establishmentAddress.Number, out var parsedAddressNumber))
            addressNumber = parsedAddressNumber;

        var address = Address.Create(addressType.Value.ID, city.ID, establishmentAddress.StreetName, addressNumber, establishmentAddress.AdditionalAddressInfo, establishmentAddress.District, zipCode, establishmentAddress.State);

        var addressID = await addressRepository.Insert(address, cancellationToken);

        var setIdResult = address.SetID(addressID);
        if (setIdResult.IsFailure)
            return Result.Failure<Address>(setIdResult.Error);

        return Result.Success(address);
    }
}
