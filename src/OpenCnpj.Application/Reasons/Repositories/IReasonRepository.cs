using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Reasons.Repositories;
public interface IReasonRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(Reason reason, CancellationToken cancellationToken);
}