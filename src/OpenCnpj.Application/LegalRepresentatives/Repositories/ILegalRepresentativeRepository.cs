using OpenCnpj.Application.LegalRepresentatives.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.LegalRepresentatives.Repositories;
public interface ILegalRepresentativeRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<LegalRepresentative> legalRepresentatives, CancellationToken cancellationToken);
}