using OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
public interface ILegalNatureRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(LegalNature legalNature, CancellationToken cancellationToken);
    Task<LegalNature?> GetByCode(string code, CancellationToken cancellationToken);
    Task<IEnumerable<LegalNature>> GetAll(CancellationToken cancellationToken);
}