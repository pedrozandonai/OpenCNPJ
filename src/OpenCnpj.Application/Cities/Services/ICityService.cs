using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Cities.Services;
public interface ICityService
{
    Task<Result> CreateCities(CancellationToken cancellationToken);
}