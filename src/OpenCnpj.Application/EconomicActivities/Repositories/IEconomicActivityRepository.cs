using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.EconomicActivities.Repositories;
public interface IEconomicActivityRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken);
    Task<EconomicActivity?> GetByCode(string code, CancellationToken cancellationToken);
    Task<IEnumerable<EconomicActivity>> GetAll(CancellationToken cancellationToken);
}