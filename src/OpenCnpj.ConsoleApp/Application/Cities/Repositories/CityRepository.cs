using Dapper;
using OpenCnpj.ConsoleApp.Application.Cities.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Cities.Repositories;
public class CityRepository(IDatabaseFactory databaseFactory) : ICityRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(City city, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO cities (code,
                                                 description)
                                         VALUES (@Code,
                                                 @Description)";

        var command = new CommandDefinition(sql, city, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<City?> GetByCode(string code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM cities
                             WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<City>(command);
    }
}
