using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class EconomicActivityRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IEconomicActivityRepository
{
    public async Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken)
        => await openCnpjDbContext.EconomicActivities.AddAsync(economicActivity, cancellationToken);

    public async Task Insert(IEnumerable<EconomicActivity> economicActivities, CancellationToken cancellationToken)
        => await openCnpjDbContext.EconomicActivities.AddRangeAsync(economicActivities, cancellationToken);

    public async Task<IEnumerable<EconomicActivity>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.EconomicActivities.ToListAsync(cancellationToken);

    public async Task<EconomicActivity?> GetByCode(string code, CancellationToken cancellationToken)
        => await openCnpjDbContext.EconomicActivities
        .Where(l => l.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefaultAsync(cancellationToken);
}
