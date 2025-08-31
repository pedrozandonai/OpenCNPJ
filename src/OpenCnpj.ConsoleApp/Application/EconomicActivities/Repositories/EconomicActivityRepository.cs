using Dapper;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Repositories;
public class EconomicActivityRepository(IDatabaseFactory databaseFactory) : IEconomicActivityRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO economic_activities (description)
                                                      VALUES (@Description)";

        var command = new CommandDefinition(sql, economicActivity, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }
}
