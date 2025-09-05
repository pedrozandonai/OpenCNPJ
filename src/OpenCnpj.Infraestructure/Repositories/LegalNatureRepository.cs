using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class LegalNatureRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ILegalNatureRepository
{
    public async Task Insert(IEnumerable<LegalNature> legalNatures, CancellationToken cancellationToken)
        => await openCnpjDbContext.LegalNatures.AddRangeAsync(legalNatures, cancellationToken);

    public async Task<IEnumerable<LegalNature>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.LegalNatures.ToListAsync(cancellationToken);

    public async Task<LegalNature?> GetByCode(string code, CancellationToken cancellationToken)
        => await openCnpjDbContext.LegalNatures
        .Where(l => l.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase))
        .FirstOrDefaultAsync(cancellationToken);
}
