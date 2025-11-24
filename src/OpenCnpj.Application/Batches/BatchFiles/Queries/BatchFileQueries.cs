using Dapper;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.BatchFiles.Queries;
public class BatchFileQueries(IDatabaseFactory databaseFactory) : IBatchFileQueries
{
    public async Task<bool> BatchFileExistsByFilePath(string filePath, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT COUNT (1)
                               FROM batch_files
                              WHERE file_path = @filePath";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, new { filePath }, cancellationToken: cancellationToken);

        return await conn.QueryFirstOrDefaultAsync<int>(command) > 0;
    }
}
