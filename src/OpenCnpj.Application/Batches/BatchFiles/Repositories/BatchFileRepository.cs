using Dapper;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Batches.BatchFiles.Repositories;
public class BatchFileRepository(IDatabaseFactory databaseFactory) : IBatchFileRepository
{
    public async Task<int> Insert(BatchFile batchFile, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO batch_files (parent_batch_file_id,
                                                      batch_id,
                                                      url,
                                                      extension,
                                                      file_name,
                                                      file_path,
                                                      file_operation,
                                                      operation_status,
                                                      type,
                                                      operation_failure_description,
                                                      is_file_deleted,
                                                      created_at)
                                              VALUES (@ParentBatchFileID,
                                                      @BatchID,
                                                      @Url,
                                                      @Extension,
                                                      @FileName,
                                                      @FilePath,
                                                      @FileOperation,
                                                      @OperationStatus,
                                                      @Type,
                                                      @OperationFailureDescription,
                                                      @IsFileDeleted,
                                                      @CreatedAt)
                                           RETURNING id";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, batchFile, cancellationToken: cancellationToken);

        return await conn.ExecuteScalarAsync<int>(command);
    }

    public async Task Update(BatchFile batchFile, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE batch_files
                                SET file_name = @FileName,
                                    file_path = @FilePath,
                                    file_operation = @FileOperation,
                                    operation_status = @OperationStatus,
                                    operation_failure_description = @OperationFailureDescription,
                                    is_file_deleted = @IsFileDeleted
                              WHERE id = @ID";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, batchFile, cancellationToken: cancellationToken);

        await conn.ExecuteAsync(command);
    }

    public async Task<BatchFile?> GetByID(int id, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    parent_batch_file_id AS ParentBatchFileID,
                                    batch_id AS BatchID,
                                    url AS Url,
                                    extension AS Extension,
                                    file_name AS FileName,
                                    file_path AS FilePath,
                                    file_operation AS FileOperation,
                                    operation_status AS OperationStatus,
                                    type AS Type,
                                    operation_failure_description AS OperationFailureDescription,
                                    is_file_deleted AS IsFileDeleted,
                                    created_at AS CreatedAt
                               FROM batch_files
                              WHERE id = @id";

        using var conn = await databaseFactory.CreateConnectionAsync();

        var command = new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken);

        return await conn.QueryFirstOrDefaultAsync<BatchFile>(command);
    }
}
