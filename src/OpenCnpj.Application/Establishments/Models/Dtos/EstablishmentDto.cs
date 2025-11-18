using OpenCnpj.Application.Addresses.Models.Dtos;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.CompanySituations.Models.Dtos;
using OpenCnpj.Application.CompanyTypes.Models.Dtos;
using OpenCnpj.Application.Contacts.Models.Dtos;
using OpenCnpj.Application.EconomicActivities.Models.Dtos;
using OpenCnpj.Application.Reasons.Models.Dtos;
using OpenCnpj.Application.SpecialSituations.Models.Dtos;

namespace OpenCnpj.Application.Establishments.Models.Dtos;
public record EstablishmentDto(CnpjDto Cnpj, string CnpjVerifierDigits, CompanyTypeDto CompanyType, string TradeName, CompanySituationDto CompanySituation, DateTime RegistratiionDateTime, ReasonDto SituationReason, string? ForeignCityName, AddressDto Address, DateTime StartActivityDate, EconomicActivityDto MainEconomicActivity, IEnumerable<EconomicActivityDto> SecondaryEconomicActivities, ContactDto Contact, SpecialSituationDto? SpecialSituation);
