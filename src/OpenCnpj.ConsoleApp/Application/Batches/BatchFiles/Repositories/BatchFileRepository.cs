using Dapper;
using OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Repositories;
public class BatchFileRepository(IDatabaseFactory databaseFactory) : IBatchFileRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(BatchFile batchFile, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batch_files (batch_id,
                                                      file_name,
                                                      file_path,
                                                      file_status_id)
                                              VALUES (@BatchID,
                                                      @FileName,
                                                      @FilePath,
                                                      @FileStatusID)";

        var command = new CommandDefinition(sql, batchFile, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task Update(BatchFile batchFile, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batch_files 
                                SET file_name = @FileName,
                                    file_path = @FilePath
                                    file_status_id = @FileStatusID
                              WHERE id = @ID";

        var command = new CommandDefinition(sql, batchFile, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<BatchFile?> Get(int batchID, string fileName, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    batch_id AS BatchID,
                                    file_name AS FileName,
                                    file_path AS FilePath,
                                    file_status_id AS FileStatusID
                               FROM batch_files
                              WHERE batch_id = @batchID
                                AND file_name = @fileName";

        var command = new CommandDefinition(sql, new { batchID, fileName }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<BatchFile>(command);
    }
}
