using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.CompanySpecialSituations.Repositories;
public interface ICompanySpecialSituationRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(CompanySpecialSituation companySpecialSituation, CancellationToken cancellationToken);
    Task Insert(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken);
    Task CopyToTable(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken);
}