using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.CompanySizes.Models.Dtos;
using OpenCnpj.Application.Establishments.Models.Dtos;
using OpenCnpj.Application.LegalNatures.Models.Dtos;
using OpenCnpj.Application.Partners.Models.Dtos;
using OpenCnpj.Application.PartnersQualifications.Models.Dtos;

namespace OpenCnpj.Application.Companies.Models.Dtos;
public record CompanyDto(CnpjDto Cnpj, string Name, LegalNatureDto LegalNature, PartnerQualificationDto PartnerQualification, decimal ShareCapital, CompanySizeDto CompanySize, string? ResponsibleFederativeEntity, bool IsSimple, bool IsMei, IEnumerable<PartnerDto> Partners, IEnumerable<EstablishmentDto> Establishments)
{
}
