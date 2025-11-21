using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Companies.CompanySizes.Models.Dtos;
using OpenCnpj.Application.Establishments.Models.Dtos;
using OpenCnpj.Application.LegalNatures.Models.Dtos;
using OpenCnpj.Application.Partners.Models.Dtos;
using OpenCnpj.Application.Qualifications.Models.Dtos;
using OpenCnpj.Application.Simples.Models.Dtos;

namespace OpenCnpj.Application.Companies.Companies.Models.Dtos;
public record CompanyDto(CnpjDto Cnpj, string Name, LegalNatureDto LegalNature, QualificationDto PartnerQualification, decimal ShareCapital, CompanySizeDto CompanySize, string? ResponsibleFederativeEntity, SimplesDto Simples, IEnumerable<PartnerDto> Partners, IEnumerable<EstablishmentDto> Establishments)
{
}
