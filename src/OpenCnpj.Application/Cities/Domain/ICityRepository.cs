using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Cities.Domain;
public interface ICityRepository : IBaseRepository
{
    Task<City?> GetByCode(long code, CancellationToken cancellationToken);
    Task<IEnumerable<City>> GetAll(CancellationToken cancellationToken);
    Task Insert(City city, CancellationToken cancellationToken);
    Task Insert(IEnumerable<City> cities, CancellationToken cancellationToken);
}
