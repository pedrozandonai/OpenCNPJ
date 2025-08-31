using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.Cities.Services;
public interface ICityService
{
    Task<Result> CreateCities(CancellationToken cancellationToken);
}