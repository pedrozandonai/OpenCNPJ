using OpenCnpj.Application.Cities.Models.Dtos;

namespace OpenCnpj.Application.Addresses.Models.Dtos;
public record AddressDto(CityDto? City, string StreetType, string StreetName, string Number, string District, string ZipCode, string State, string? AdditionalInformation);
