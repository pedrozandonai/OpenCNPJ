using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OpenCnpj.Core.Database;
public interface IBaseRepository
{
    public DatabaseFacade Database { get; }
    Task SaveAllChanges(CancellationToken cancellationToken = default);
}
