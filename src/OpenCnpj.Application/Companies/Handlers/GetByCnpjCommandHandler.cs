using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Commands;
using OpenCnpj.Application.Companies.Models.Dtos;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Companies.Handlers;
public class GetByCnpjCommandHandler : IRequestHandler<GetByCnpjCommand, Result<CompanyDto?>>
{
    public async Task<Result<CompanyDto?>> Handle(GetByCnpjCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
