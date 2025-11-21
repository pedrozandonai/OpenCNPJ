using Dapper;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.Batches.Queries;

public class BatchQueries(IDatabaseFactory databaseFactory) : IBatchQueries
{
    public async Task<bool> BatchExistsByID(int id, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT COUNT (1)
                               FROM batches
                              WHERE id = @id";

        var command = new CommandDefinition(sql, new { id }, transaction: databaseFactory.Transaction, cancellationToken: cancellationToken);

        return await databaseFactory.Connection.QueryFirstOrDefaultAsync<int>(command) > 0;
    }
}
