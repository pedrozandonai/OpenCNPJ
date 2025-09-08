using OpenCnpj.Application.Phones.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Phones.Repositories;
public interface IPhoneRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<Phone> phones, CancellationToken cancellationToken);
}