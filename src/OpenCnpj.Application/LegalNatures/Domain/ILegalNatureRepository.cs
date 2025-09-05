using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.LegalNatures.Domain;
public interface ILegalNatureRepository : IBaseRepository
{
    Task Insert(IEnumerable<LegalNature> legalNatures, CancellationToken cancellationToken);
    Task<IEnumerable<LegalNature>> GetAll(CancellationToken cancellationToken);
    Task<LegalNature?> GetByCode(string code, CancellationToken cancellationToken);
}
