using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Repositories;
public interface ICompanySecondaryEconomicActivityRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<CompanySecondaryEconomicActivity> companySecondaryEconomicActivities, CancellationToken cancellationToken);
}