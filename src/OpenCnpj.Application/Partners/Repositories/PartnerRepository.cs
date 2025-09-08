using Npgsql;
using OpenCnpj.Application.Partners.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Partners.Repositories;
public class PartnerRepository(IDatabaseFactory databaseFactory) : IPartnerRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task CopyToTable(IEnumerable<Partner> partners, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY partners (id,
                                            company_id,
                                            country_id,
                                            partner_type_id,
                                            legal_representative_id,
                                            qualification_id,
                                            age_range_id,
                                            name,
                                            identifier,
                                            start_date)
                                FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var partner in partners)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(partner.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(partner.CompanyID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                long? countryID = partner.CountryID.HasValue ? partner.CountryID.Value : null;
                await writer.WriteAsync(countryID ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(partner.PartnerTypeID, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                long? legalRepresentativeID = partner.LegalRepresentativeID.HasValue ? partner.LegalRepresentativeID.Value : null;
                await writer.WriteAsync(legalRepresentativeID ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(partner.QualificationID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(partner.AgeRangeID, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(partner.Name, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(partner.Identifier, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(partner.StartDate, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing partner at row {rowCount}, ID: {partner.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
