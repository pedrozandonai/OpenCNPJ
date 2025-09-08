using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Companies.Repositories;
public interface ICompanyRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(IEnumerable<Company> companies, CancellationToken cancellationToken);
    Task CopyToTable(IEnumerable<Company> companies, CancellationToken cancellationToken);
    Task<Company?> GetByBasicCnpj(string basicCnpj, CancellationToken cancellationToken);
}