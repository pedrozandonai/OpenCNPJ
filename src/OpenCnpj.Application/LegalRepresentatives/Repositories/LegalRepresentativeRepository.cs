using Npgsql;
using OpenCnpj.Application.LegalRepresentatives.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.LegalRepresentatives.Repositories;
public class LegalRepresentativeRepository(IDatabaseFactory databaseFactory) : ILegalRepresentativeRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task CopyToTable(IEnumerable<LegalRepresentative> legalRepresentatives, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY legal_representatives (id,
                                                         qualification_id,
                                                         identifier,
                                                         name)
                                             FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var legalRepresentative in legalRepresentatives)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(legalRepresentative.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(legalRepresentative.QualificationID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(legalRepresentative.Identifier, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(legalRepresentative.Name, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing legalRepresentative at row {rowCount}, ID: {legalRepresentative.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
