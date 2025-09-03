using OpenCnpj.ConsoleApp.Application.Countries.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Countries.Repositories;
public interface ICountryRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(Country country, CancellationToken cancellationToken);
    Task<Country?> GetByCode(string code, CancellationToken cancellationToken);
}