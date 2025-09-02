using OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Repositories;
public interface IEconomicActivityRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken);
}