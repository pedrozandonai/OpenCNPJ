using Microsoft.EntityFrameworkCore;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;
public class CityRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), ICityRepository
{
    public async Task Insert(City city, CancellationToken cancellationToken)
        => await openCnpjDbContext.Cities.AddAsync(city, cancellationToken);

    public async Task Insert(IEnumerable<City> cities, CancellationToken cancellationToken)
        => await openCnpjDbContext.Cities.AddRangeAsync(cities, cancellationToken);

    public async Task<IEnumerable<City>> GetAll(CancellationToken cancellationToken)
        => await openCnpjDbContext.Cities
        .ToListAsync(cancellationToken);

    public async Task<City?> GetByCode(long code, CancellationToken cancellationToken)
        => await openCnpjDbContext.Cities
        .AsNoTracking()
        .Where(c => c.Code == code)
        .FirstOrDefaultAsync(cancellationToken);
}
