using OpenCnpj.ConsoleApp.Application.CompanySpecialSituations.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.CompanySpecialSituations.Repositories;
public interface ICompanySpecialSituationRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(CompanySpecialSituation companySpecialSituation, CancellationToken cancellationToken);
    Task Insert(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken);
}