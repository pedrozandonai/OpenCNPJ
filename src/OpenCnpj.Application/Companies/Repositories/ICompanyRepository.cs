using OpenCnpj.Application.Companies.Domain;

namespace OpenCnpj.Application.Companies.Repositories;
public interface ICompanyRepository
{
    Task CopyToTable(IEnumerable<Company> companies, CancellationToken cancellationToken);
    Task<Company?> GetByBasicCnpj(string basicCnpj, CancellationToken cancellationToken);
}