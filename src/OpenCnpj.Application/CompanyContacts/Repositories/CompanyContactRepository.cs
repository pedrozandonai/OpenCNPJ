using Npgsql;
using OpenCnpj.Application.CompanyContacts.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.CompanyContacts.Repositories;
public class CompanyContactRepository(IDatabaseFactory databaseFactory) : ICompanyContactRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;


    public async Task CopyToTable(IEnumerable<CompanyContact> companyContacts, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY company_contacts (company_id,
                                                    contact_id)
                                      FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;

        foreach (var companyContact in companyContacts)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(companyContact.CompanyID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(companyContact.ContactID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing companyContact at row {rowCount}, CompanyID: {companyContact.CompanyID}, ContactID: {companyContact.ContactID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}

