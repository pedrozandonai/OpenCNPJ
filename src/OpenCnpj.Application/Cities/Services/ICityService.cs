using CSharpFunctionalExtensions;
using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Cities.Services;
public interface ICityService
{
    Task<Result> CreateCities(CancellationToken cancellationToken);
}