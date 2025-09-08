using Npgsql;
using OpenCnpj.Application.Meis.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Meis.Repositories;
public class MeiRepository(IDatabaseFactory databaseFactory) : IMeiRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task CopyToTable(IEnumerable<Mei> meis, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY meis (id,
                                        date_opted,
                                        exclusion_date)
                            FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var mei in meis)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(mei.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(mei.DateOpted ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
                await writer.WriteAsync(mei.ExclusionDate ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing company at row {rowCount}, ID: {mei.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
