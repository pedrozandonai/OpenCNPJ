using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Models.Dtos;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Companies.Commands;
public record GetByCnpjCommand : IRequest<Result<CompanyDto?>>
{
    public string Cnpj { get; init; }

    public GetByCnpjCommand(string cnpj)
    {
        Cnpj = cnpj;
    }
}
