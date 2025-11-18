namespace OpenCnpj.Application.Addresses.Models.Dtos;
public record AddressDto(string StreetType, string StreetName, string Number, string District, string ZipCode, string State, string? AdditionalInformation);
