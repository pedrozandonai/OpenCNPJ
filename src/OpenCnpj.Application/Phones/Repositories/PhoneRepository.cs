using Npgsql;
using OpenCnpj.Application.Phones.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Phones.Repositories;
public class PhoneRepository(IDatabaseFactory databaseFactory) : IPhoneRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;


    public async Task CopyToTable(IEnumerable<Phone> phones, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY phones (id,
                                          area_code,
                                          phone)
                              FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var phone in phones)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(phone.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(phone.AreaCode, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(phone.PhoneNumber, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing phone at row {rowCount}, ID: {phone.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
