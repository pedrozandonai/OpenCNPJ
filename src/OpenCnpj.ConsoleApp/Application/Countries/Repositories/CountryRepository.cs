using Dapper;
using OpenCnpj.ConsoleApp.Application.Countries.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Countries.Repositories;
public class CountryRepository(IDatabaseFactory databaseFactory) : ICountryRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(Country country, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO countries (code,
                                                    description)
                                            VALUES (@Code,
                                                    @Description)";

        var command = new CommandDefinition(sql, country, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<Country?> GetByCode(string code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM countries
                             WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<Country>(command);
    }

    public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM countries";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<Country>(command);
    }
}
