using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Cities.Repositories;
public interface ICityRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(City city, CancellationToken cancellationToken);
    Task<City?> GetByCode(long code, CancellationToken cancellationToken);
    Task<IEnumerable<City>> GetAll(CancellationToken cancellationToken);
}