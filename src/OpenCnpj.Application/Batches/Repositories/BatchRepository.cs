using Dapper;
using OpenCnpj.Application.Batches.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.Repositories;
public class BatchRepository(IDatabaseFactory databaseFactory) : IBatchRepository
{
    public async Task<int> Insert(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batches (identifier,
                                                  operation,
                                                  operation_status,
                                                  operation_failure_description,
                                                  directory,
                                                  retry_date)
                                          VALUES (@Identifier,
                                                  @Operation,
                                                  @OperationStatus,
                                                  @OperationFailureDescription,
                                                  @Directory,
                                                  @RetryDate)
                                       RETURNING id";

        var command = new CommandDefinition(sql, batch, transaction: databaseFactory.Transaction, cancellationToken:cancellationToken);

        return await databaseFactory.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task Update(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batches
                                SET identifier = @Identifier,
                                    operation = @Operation,
                                    operation_status = @OperationStatus,
                                    operation_failure_description = @OperationFailureDescription,
                                    directory = @Directory,
                                    retry_date = @RetryDate
                              WHERE id = @ID";

        var command = new CommandDefinition(sql, batch, transaction: databaseFactory.Transaction, cancellationToken: cancellationToken);

        await databaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<Batch?> GetBatchByIdentifier(string identifier, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    identifier AS Identifier,
                                    operation AS Operation,
                                    operation_status AS OperationStatus,
                                    operation_failure_description AS OperationFailureDescription,
                                    directory AS Directory,
                                    retry_date AS RetryDate
                               FROM batches
                              WHERE identifier = @identifier";

        var command = new CommandDefinition(sql, new { identifier }, transaction: databaseFactory.Transaction, cancellationToken: cancellationToken);

        return await databaseFactory.Connection.QueryFirstOrDefaultAsync<Batch>(command);
    }
}
