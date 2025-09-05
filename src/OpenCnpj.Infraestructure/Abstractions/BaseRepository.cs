using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenCnpj.Core.Database;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Abstractions;
public abstract class BaseRepository(OpenCnpjDbContext dbContext) : IBaseRepository
{
    public DatabaseFacade Database => dbContext.Database;

    public virtual async Task SaveAllChanges(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

