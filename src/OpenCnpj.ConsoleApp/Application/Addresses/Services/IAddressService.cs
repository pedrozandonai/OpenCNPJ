using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Addresses.Domain;
using OpenCnpj.ConsoleApp.Application.RawRecords;

namespace OpenCnpj.ConsoleApp.Application.Addresses.Services;
public interface IAddressService
{
    Task<Result<Address>> CreateAddressByEstablishmentRawRecord(EstablishmentRawRecord establishmentRawRecord, CancellationToken cancellationToken);
}