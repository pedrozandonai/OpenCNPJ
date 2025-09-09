namespace OpenCnpj.Core.Database.Services;

public interface IPgBulkCopyService
{
    Task CopyAsync<T>(string connectionString, IEnumerable<T> items, int batchSize, CancellationToken ct);
}
