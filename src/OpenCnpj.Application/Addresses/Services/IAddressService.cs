using CSharpFunctionalExtensions;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.Application.Addresses.Services;
public interface IAddressService
{
    Task<Result<Address>> CreateAddressByEstablishmentRawRecord(EstablishmentRawRecord establishmentRawRecord, CancellationToken cancellationToken);
}