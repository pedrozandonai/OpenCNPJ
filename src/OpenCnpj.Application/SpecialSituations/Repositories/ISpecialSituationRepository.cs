using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.SpecialSituations.Repositories;

public interface ISpecialSituationRepository : IOpenCnpjDatabaseFactory
{
    Task<int> Insert(SpecialSituation specialSituation, CancellationToken cancellationToken);
    Task Insert(IEnumerable<SpecialSituation> specialSituations, CancellationToken cancellationToken);
    Task<SpecialSituation?> GetByDescription(string description, CancellationToken cancellationToken);
    Task<IEnumerable<SpecialSituation>> GetAll(CancellationToken cancellationToken);
}