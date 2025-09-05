using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Companies.Services;
public interface ICompanyService
{
    Task<Result> CreateCompanies(CancellationToken cancellationToken);
}