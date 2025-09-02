using Dapper;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Repositories;
public class BatchRepository(IDatabaseFactory databaseFactory) : IBatchRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<int> Insert(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batches (identifier,
                                                  status,
                                                  application_last_step_id)
                                          VALUES (@Identifier,
                                                  @Status,
                                                  @ApplicationLastStepID)
                                       RETURNING id";

        var command = new CommandDefinition(sql, batch, transaction:DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task Update(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batches
                                SET status = @Status
                              WHERE id = @ID";

        var command = new CommandDefinition(sql, batch, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    identifier AS Identifier,
                                    status AS Status,
                                    application_last_step_id AS ApplicationLastStepId
                               FROM batches
                              WHERE identifier = @Identifier";

        var command = new CommandDefinition(sql, new { identifier }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<Batch>(command);
    }
}
