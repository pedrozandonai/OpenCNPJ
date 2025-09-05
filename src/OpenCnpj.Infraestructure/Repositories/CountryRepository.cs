using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class CountryRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ICountryRepository
{
    public async Task Insert(IEnumerable<Country> countries, CancellationToken cancellationToken)
        => await openCnpjDbContext.Countries.AddRangeAsync(countries, cancellationToken);

    public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.Countries.ToListAsync(cancellationToken);

    public async Task<Country?> GetByCode(string code, CancellationToken cancellationToken)
        => await openCnpjDbContext.Countries
        .Where(l => l.Code.Equals(code))
        .FirstOrDefaultAsync(cancellationToken);
}
