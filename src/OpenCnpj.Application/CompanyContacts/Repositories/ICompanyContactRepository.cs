using OpenCnpj.Application.CompanyContacts.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.CompanyContacts.Repositories;
public interface ICompanyContactRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<CompanyContact> companyContacts, CancellationToken cancellationToken);
}