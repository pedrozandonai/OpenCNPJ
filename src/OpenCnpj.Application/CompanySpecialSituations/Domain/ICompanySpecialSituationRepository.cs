using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.CompanySpecialSituations.Domain;
public interface ICompanySpecialSituationRepository : IBaseRepository
{
    Task Insert(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken);
}
