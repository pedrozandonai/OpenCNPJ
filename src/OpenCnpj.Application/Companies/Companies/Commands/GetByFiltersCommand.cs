using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Companies.Models.Dtos;
using OpenCnpj.Application.Paginations.Models;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Companies.Companies.Commands;
public class GetByFiltersCommand : PaginatedRequest, IRequest<Result<PaginationViewModel<CompanyDto>>>
{
    public string? CompanyName { get; init; }
    public string? FullCnpj { get; init; }
    public string? BaseCnpj { get; init; }
    public int? LegalNatureCode { get; init; }
    public int? ResponsibleQualification { get; init; }
    public int? ShareCapital { get; init; }
    public short? CompanySize { get; init; }
    //TODO: Esses filtros não são da coleção de 'empresas' e por isso, precisa de uma lógica diferente pra filtrar por eles. Implementar no futuro.
    //public string? TradeName { get; init; }
    //public int? RegistrationStatus { get; init; }
    //public int? Cnae { get; init; }
    //public string? StreetName { get; init; }
    //public string? AddressNumber { get; init; }
    //public string? AddressDistrict { get; init; }
    //public string? AddressState { get; init; }
    //public string? ZipCode { get; init; }
}
