using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.Companies.Services;
public interface ICompanyService
{
    Task<Result> CreateCompanies(CancellationToken cancellationToken);
}