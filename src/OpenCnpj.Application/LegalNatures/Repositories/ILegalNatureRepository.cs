using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.LegalNatures.Repositories;
public interface ILegalNatureRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(LegalNature legalNature, CancellationToken cancellationToken);
    Task<LegalNature?> GetByCode(string code, CancellationToken cancellationToken);
    Task<IEnumerable<LegalNature>> GetAll(CancellationToken cancellationToken);
}