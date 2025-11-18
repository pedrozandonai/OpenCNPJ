using OpenCnpj.Application.Ages.Models.Dtos;
using OpenCnpj.Application.Countries.Models.Dtos;
using OpenCnpj.Application.LegalRepresentatives.Models.Dtos;
using OpenCnpj.Application.PartnersQualifications.Models.Dtos;
using OpenCnpj.Application.PartnerTypes.Models.Dtos;

namespace OpenCnpj.Application.Partners.Models.Dtos;
public record PartnerDto(PartnerTypeDto Type, string Name, string Document, PartnerQualificationDto Qualification, DateTime EntryDate, CountryDto? Country, RepresentativeDto? Representative, AgeDto AgeRange);
