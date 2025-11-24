using Dapper;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.Batches.Repositories;
public class BatchRepository(IDatabaseFactory databaseFactory) : IBatchRepository
{
    public async Task<int> Insert(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batches (period,
                                                  operation,
                                                  operation_status,
                                                  operation_failure_description,
                                                  directory,
                                                  retry_date,
                                                  created_at,
                                                  finished_at)
                                          VALUES (@Period,
                                                  @Operation,
                                                  @OperationStatus,
                                                  @OperationFailureDescription,
                                                  @Directory,
                                                  @RetryDate,
                                                  @CreatedAt,
                                                  @FinishedAt)
                                       RETURNING id";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, batch, cancellationToken:cancellationToken);

        return await conn.ExecuteScalarAsync<int>(command);
    }

    public async Task Update(Batch batch, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batches
                                SET period = @Period,
                                    operation = @Operation,
                                    operation_status = @OperationStatus,
                                    operation_failure_description = @OperationFailureDescription,
                                    directory = @Directory,
                                    retry_date = @RetryDate,
                                    finished_at = @FinishedAt
                              WHERE id = @ID";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, batch, cancellationToken: cancellationToken);

        await conn.ExecuteAsync(command);
    }

    public async Task<Batch?> GetBatchByPeriod(string period, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    period AS Period,
                                    operation AS Operation,
                                    operation_status AS OperationStatus,
                                    operation_failure_description AS OperationFailureDescription,
                                    directory AS Directory,
                                    retry_date AS RetryDate,
                                    created_at AS CreatedAt,
                                    finished_at AS FinishedAt
                               FROM batches
                              WHERE period = @period";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, new { period }, cancellationToken: cancellationToken);

        return await conn.QueryFirstOrDefaultAsync<Batch>(command);
    }

    public async Task<Batch?> GetByID(int id, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    period AS Period,
                                    operation AS Operation,
                                    operation_status AS OperationStatus,
                                    operation_failure_description AS OperationFailureDescription,
                                    directory AS Directory,
                                    retry_date AS RetryDate,
                                    created_at AS CreatedAt,
                                    finished_at AS FinishedAt
                               FROM batches
                              WHERE id = @id";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken);

        return await conn.QueryFirstOrDefaultAsync<Batch>(command);
    }
}
