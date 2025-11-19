using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Models.Dtos;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Companies.Commands;
public record GetByFiltersCommand : IRequest<Result<IEnumerable<CompanyDto>>>
{
    public string? CompanyName { get; init; }
    public string? FullCnpj { get; init; }
    public string? BaseCnpj { get; init; }
    public int? LegalNatureCode { get; init; }
    public int? ResponsibleQualification { get; init; }
    public int? ShareCapital { get; init; }
    public short? CompanySize { get; init; }
    public bool? HeadOffice { get; init; }
    public string? TradeName { get; init; }
    public int? RegistrationStatus { get; init; }
    public int? Cnae { get; init; }
    public string? Cnaes { get; init; }
    public string? StreetName { get; init; }
    public string? AddressNumber { get; init; }
    public string? AddressDistrict { get; init; }
    public string? AddressState { get; init; }
    public string? ZipCode { get; init; }
}
