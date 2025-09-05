using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Infraestructure.Abstractions;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Repositories;

public class ReasonRepository(OpenCnpjDbContext openCnpjDbContext) : BaseRepository(openCnpjDbContext), IReasonRepository
{
    public async Task Insert(IEnumerable<Reason> reasons, CancellationToken cancellationToken)
        => await openCnpjDbContext.Reasons.AddRangeAsync(reasons, cancellationToken);
}
