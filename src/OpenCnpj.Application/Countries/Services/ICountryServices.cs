using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Countries.Services;
public interface ICountryServices
{
    Task<Result> CreateCountries(CancellationToken cancellationToken);
}