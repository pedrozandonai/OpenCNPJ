using Dapper;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Cities.Repositories;
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

    public async Task<City?> GetByCode(long code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM cities
                             WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<City>(command);
    }

    public async Task<IEnumerable<City>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM cities";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<City>(command);
    }
}
