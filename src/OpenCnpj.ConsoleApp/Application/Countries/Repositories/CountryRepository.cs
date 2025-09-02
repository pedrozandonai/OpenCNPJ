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
}
