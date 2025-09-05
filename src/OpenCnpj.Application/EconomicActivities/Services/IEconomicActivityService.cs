using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.EconomicActivities.Services;
public interface IEconomicActivityService
{
    Task<Result> CreateEconomicActivities(CancellationToken cancellationToken);
}