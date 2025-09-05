using Dapper;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.EconomicActivities.Repositories;
public class EconomicActivityRepository(IDatabaseFactory databaseFactory) : IEconomicActivityRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(EconomicActivity economicActivity, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO economic_activities (code,
                                                              description)
                                                      VALUES (@Code,
                                                              @Description)";

        var command = new CommandDefinition(sql, economicActivity, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<EconomicActivity?> GetByCode(string code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM economic_activities
                              WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<EconomicActivity>(command);
    }

    public async Task<IEnumerable<EconomicActivity>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM economic_activities";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<EconomicActivity>(command);
    }
}
