using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.EconomicActivities.Domain;
public interface IEconomicActivityRepository : IBaseRepository
{
    Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken);
    Task Insert(IEnumerable<EconomicActivity> economicActivities, CancellationToken cancellationToken);
    Task<IEnumerable<EconomicActivity>> GetAll(CancellationToken cancellationToken);
    Task<EconomicActivity?> GetByCode(string code, CancellationToken cancellationToken);
}
