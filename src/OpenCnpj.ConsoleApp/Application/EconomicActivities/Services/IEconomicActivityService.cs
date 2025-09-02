using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Services;
public interface IEconomicActivityService
{
    Task<Result> CreateEconomicActivities(CancellationToken cancellationToken);
}