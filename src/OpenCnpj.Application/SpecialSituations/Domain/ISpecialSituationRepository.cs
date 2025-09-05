using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.SpecialSituations.Domain;
public interface ISpecialSituationRepository : IBaseRepository
{
    Task Insert(IEnumerable<SpecialSituation> specialSituations, CancellationToken cancellationToken);
    Task<IEnumerable<SpecialSituation>> GetAll(CancellationToken cancellationToken);
}
