using OpenCnpj.Application.Contacts.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Contacts.Repositories;
public interface IContactRepository : IOpenCnpjDatabaseFactory
{
    Task CopyToTable(IEnumerable<Contact> contacts, CancellationToken cancellationToken);
}