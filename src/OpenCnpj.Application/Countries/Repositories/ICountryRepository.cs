using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Countries.Repositories;
public interface ICountryRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(Country country, CancellationToken cancellationToken);
    Task<Country?> GetByCode(string code, CancellationToken cancellationToken);
    Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken);
}