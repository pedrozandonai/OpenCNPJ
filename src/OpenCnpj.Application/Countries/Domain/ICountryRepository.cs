using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Countries.Domain;
public interface ICountryRepository : IBaseRepository
{
    Task Insert(IEnumerable<Country> countries, CancellationToken cancellationToken);
    Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken);
    Task<Country?> GetByCode(string code, CancellationToken cancellationToken);
}
