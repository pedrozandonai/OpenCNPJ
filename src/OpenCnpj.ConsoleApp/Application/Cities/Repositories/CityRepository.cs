using Dapper;
using OpenCnpj.ConsoleApp.Application.Cities.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Cities.Repositories;
public class CityRepository(IDatabaseFactory databaseFactory) : ICityRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(City city, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO cities (description)
                                         VALUES (@Description)";

        var command = new CommandDefinition(sql, city, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }
}
