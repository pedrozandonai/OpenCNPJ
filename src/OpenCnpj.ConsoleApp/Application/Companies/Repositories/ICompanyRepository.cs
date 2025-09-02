using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Companies.Repositories;
public interface ICompanyRepository : IOpenCnpjDatabaseFactory
{
    Task Insert(Company company, CancellationToken cancellationToken);
}