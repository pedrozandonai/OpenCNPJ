using OpenCnpj.Application.CompanySizes.Models.Dtos;
using OpenCnpj.Application.LegalNatures.Models.Dtos;
using OpenCnpj.Application.PartnersQualifications.Models.Dtos;

namespace OpenCnpj.Application.Companies.Models.Dtos;
public record CompanyDto(string BaseCnpj, string Name, LegalNatureDto LegalNature, PartnerQualificationDto PartnerQualification, decimal ShareCapital, CompanySizeDto CompanySize, string? ResponsibleFederativeEntity)
{
}
