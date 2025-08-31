using Dapper;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Repositories;
public class BatchRepository(IDatabaseFactory databaseFactory) : IBatchRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batches (id,
                                                  identifier,
                                                  status)
                                          VALUES (@ID,
                                                  @Identifier,
                                                  @Status)";

        var command = new CommandDefinition(sql, batch, transaction:DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task Update(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batches
                                SET status = @Status
                              WHERE id = @ID";

        var command = new CommandDefinition(sql, batch, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<int> GetSequence(CancellationToken cancellationToken)
    {
        const string sql = "SELECT nextval('seq_batches')";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<int>(command);
    }
}
