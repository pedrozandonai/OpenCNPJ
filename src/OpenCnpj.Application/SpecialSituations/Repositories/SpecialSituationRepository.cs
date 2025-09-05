using Dapper;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.SpecialSituations.Repositories;

public class SpecialSituationRepository(IDatabaseFactory databaseFactory) : ISpecialSituationRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<int> Insert(SpecialSituation specialSituation, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO special_situations (description)
                                                     VALUES (@Description)
                                                  RETURNING ID";

        var command = new CommandDefinition(sql, specialSituation, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task Insert(IEnumerable<SpecialSituation> specialSituations, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO special_situations (id,
                                                             description)
                                                     VALUES (@ID,
                                                             @Description)";

        var command = new CommandDefinition(sql, specialSituations, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<SpecialSituation?> GetByDescription(string description, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    description AS Description
                               FROM special_situations
                              WHERE description = @description";

        var command = new CommandDefinition(sql, new { description }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<SpecialSituation>(command);
    }

    public async Task<IEnumerable<SpecialSituation>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    description AS Description
                               FROM special_situations";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<SpecialSituation>(command);
    }
}