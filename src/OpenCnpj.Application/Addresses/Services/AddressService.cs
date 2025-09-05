using CSharpFunctionalExtensions;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.Addresses.Repositories;
using OpenCnpj.Application.AddressTypes.Services;
using OpenCnpj.Application.Cities.Repositories;
using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.Application.Addresses.Services;
public class AddressService(IAddressTypeService addressTypeService, ICityRepository cityRepository, IAddressRepository addressRepository) : IAddressService
{
    public async Task<Result<Address>> CreateAddressByEstablishmentRawRecord(EstablishmentRawRecord establishmentRawRecord, CancellationToken cancellationToken)
    {
        var establishmentAddress = establishmentRawRecord.Address;

        var addressType = await addressTypeService.GetOrCreateAddressTypeByStreetType(establishmentAddress.StreetType, cancellationToken);
        if (addressType.IsFailure)
            return Result.Failure<Address>(addressType.Error);

        if (!long.TryParse(establishmentAddress.MunicipalityCode, out var cityCode))
            return Result.Failure<Address>("Unable to parse MunicipalityCode");

        var city = await cityRepository.GetByCode(cityCode, cancellationToken);
        if (city == null)
            return Result.Failure<Address>("The city of the establishment could not be retreived.");

        int? zipCode = null;
        if (int.TryParse(establishmentAddress.ZipCode, out var parsedZipCode))
            zipCode = parsedZipCode;

        int? addressNumber = null;
        if (int.TryParse(establishmentAddress.Number, out var parsedAddressNumber))
            addressNumber = parsedAddressNumber;

        var address = Address.Create(0, addressType.Value.ID, city.ID, establishmentAddress.StreetName, addressNumber, establishmentAddress.AdditionalAddressInfo, establishmentAddress.District, zipCode, establishmentAddress.State);

        var addressID = await addressRepository.Insert(address, cancellationToken);

        var setIdResult = address.SetID(addressID);
        if (setIdResult.IsFailure)
            return Result.Failure<Address>(setIdResult.Error);

        return Result.Success(address);
    }
}
