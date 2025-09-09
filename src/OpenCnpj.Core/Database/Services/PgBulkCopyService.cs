using Npgsql;
using OpenCnpj.Core.Mappers;
using OpenCnpj.Core.Models;

namespace OpenCnpj.Core.Database.Services;
public sealed class PgBulkCopyService : IPgBulkCopyService
{
    public async Task CopyAsync<T>(string connectionString, IEnumerable<T> items, int batchSize, CancellationToken ct)
    {
        var (table, cols) = PgMappingCache.GetMapping<T>();
        if (cols.Length == 0) return;

        var columnList = string.Join(", ", cols.Select(c => $"\"{c.ColumnName}\""));
        var copySql = $"copy \"{table}\" ({columnList}) from stdin (format binary)";

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);

        // processa em chunks para não estourar memória
        var batch = new List<T>(capacity: Math.Max(1, batchSize));
        foreach (var item in items)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                await CopyChunk(conn, copySql, cols, batch, ct);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await CopyChunk(conn, copySql, cols, batch, ct);
    }

    private static async Task CopyChunk<T>(NpgsqlConnection conn, string copySql, PgColumnDescriptor[] cols, List<T> chunk, CancellationToken ct)
    {
        await using var writer = await conn.BeginBinaryImportAsync(copySql, ct);

        foreach (var row in chunk)
        {
            await writer.StartRowAsync(ct);

            foreach (var col in cols)
            {
                var val = col.Property.GetValue(row);
                if (val == null)
                {
                    await writer.WriteNullAsync(ct);
                }
                else
                {
                    var underlying = Nullable.GetUnderlyingType(col.Property.PropertyType);
                    if ((underlying?.IsEnum ?? col.Property.PropertyType.IsEnum))
                    {
                        var intVal = (int)Convert.ChangeType(val, typeof(int));
                        await writer.WriteAsync(intVal, col.DbType, ct);
                    }
                    else
                    {
                        await writer.WriteAsync(val, col.DbType, ct);
                    }
                }
            }
        }

        await writer.CompleteAsync(ct);
    }
}