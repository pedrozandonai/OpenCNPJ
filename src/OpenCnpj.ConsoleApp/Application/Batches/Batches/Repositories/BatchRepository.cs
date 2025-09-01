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
                                                  status)
                                          VALUES (@Identifier,
                                                  @Status)
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
}
