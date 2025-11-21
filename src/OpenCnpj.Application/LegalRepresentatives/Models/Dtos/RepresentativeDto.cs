using OpenCnpj.Application.Qualifications.Models.Dtos;

namespace OpenCnpj.Application.LegalRepresentatives.Models.Dtos;

public record RepresentativeDto(string Document, string Name, QualificationDto Qualification);
