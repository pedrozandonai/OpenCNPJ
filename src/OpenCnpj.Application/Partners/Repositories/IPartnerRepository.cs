using OpenCnpj.Application.Partners.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Partners.Repositories;
public interface IPartnerRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<Partner> partners, CancellationToken cancellationToken);
}