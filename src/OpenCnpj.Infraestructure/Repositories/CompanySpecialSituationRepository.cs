using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class CompanySpecialSituationRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ICompanySpecialSituationRepository
{
    public async Task Insert(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken)
        => await openCnpjDbContext.CompanySpecialSituations.AddRangeAsync(companySpecialSituations, cancellationToken);
}
