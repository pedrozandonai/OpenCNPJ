using OpenCnpj.Core.Database;

namespace OpenCnpj.Application.Reasons.Domain;
public interface IReasonRepository : IBaseRepository
{
    Task Insert(IEnumerable<Reason> reasons, CancellationToken cancellationToken);
}
