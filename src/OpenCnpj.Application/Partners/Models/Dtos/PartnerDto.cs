using OpenCnpj.Application.Ages.Models.Dtos;
using OpenCnpj.Application.Countries.Models.Dtos;
using OpenCnpj.Application.LegalRepresentatives.Models.Dtos;
using OpenCnpj.Application.PartnerTypes.Models.Dtos;
using OpenCnpj.Application.Qualifications.Models.Dtos;

namespace OpenCnpj.Application.Partners.Models.Dtos;
public record PartnerDto(PartnerTypeDto Type, string Name, string Document, QualificationDto Qualification, DateTime? EntryDate, CountryDto? Country, RepresentativeDto? Representative, AgeDto AgeRange);
