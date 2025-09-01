using OpenCnpj.ConsoleApp.Application.Reasons.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Reasons.Repositories;
public interface IReasonRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(Reason reason, CancellationToken cancellationToken);
}