using Npgsql;
using OpenCnpj.Application.Simples.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Simples.Repositories;
public class SimpleRepository(IDatabaseFactory databaseFactory) : ISimpleRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task CopyToTable(IEnumerable<Simple> simples, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY simples (id,
                                           company_id,
                                           mei_id,
                                           is_simple,
                                           date_opted,
                                           exclusion_date)
                               FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var simple in simples)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(simple.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(simple.CompanyID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(simple.MeiID ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(simple.IsSimple ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Boolean, cancellationToken);
                await writer.WriteAsync(simple.DateOpted ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
                await writer.WriteAsync(simple.ExclusionDate ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing company at row {rowCount}, ID: {simple.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}