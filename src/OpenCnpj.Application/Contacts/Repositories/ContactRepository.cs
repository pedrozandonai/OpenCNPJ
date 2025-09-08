using Npgsql;
using OpenCnpj.Application.Contacts.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Contacts.Repositories;
public class ContactRepository(IDatabaseFactory databaseFactory) : IContactRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;


    public async Task CopyToTable(IEnumerable<Contact> contacts, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY contacts (id,
                                            phone_id,
                                            fax_area_code,
                                            fax_number,
                                            email_address)
                              FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var contact in contacts)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(contact.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(contact.PhoneID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(contact.FaxAreaCode, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(contact.FaxNumber, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(contact.EmailAddress ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing contact at row {rowCount}, ID: {contact.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
