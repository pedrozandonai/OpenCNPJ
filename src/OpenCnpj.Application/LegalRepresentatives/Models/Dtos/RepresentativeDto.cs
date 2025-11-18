using OpenCnpj.Application.PartnersQualifications.Models.Dtos;

namespace OpenCnpj.Application.LegalRepresentatives.Models.Dtos;

public record RepresentativeDto(string Document, string Name, PartnerQualificationDto Qualification);
