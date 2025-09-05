using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Companies.Domain;
public interface ICompanyRepository : IBaseRepository
{
    Task Insert(IEnumerable<Company> companies, CancellationToken cancellationToken);
}
