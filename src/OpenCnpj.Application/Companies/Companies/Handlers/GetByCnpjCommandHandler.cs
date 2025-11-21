using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Companies.Commands;
using OpenCnpj.Application.Companies.Companies.Models.Dtos;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Companies.Companies.Handlers;
public class GetByCnpjCommandHandler : IRequestHandler<GetByCnpjCommand, Result<CompanyDto?>>
{
    public async Task<Result<CompanyDto?>> Handle(GetByCnpjCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
