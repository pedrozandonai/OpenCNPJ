using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class SpecialSituationRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ISpecialSituationRepository
{
    public async Task Insert(IEnumerable<SpecialSituation> specialSituations, CancellationToken cancellationToken)
        => await openCnpjDbContext.SpecialSituations
        .AddRangeAsync(specialSituations, cancellationToken);

    public async Task<IEnumerable<SpecialSituation>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.SpecialSituations
        .ToListAsync(cancellationToken);
}
