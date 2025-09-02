using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.Countries.Services;
public interface ICountryServices
{
    Task<Result> CreateCountries(CancellationToken cancellationToken);
}