using Dapper;
using Npgsql;
using OpenCnpj.ConsoleApp.Application.Addresses.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Addresses.Repositories;
public class AddressRepository(IDatabaseFactory databaseFactory) : IAddressRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<long> Insert(Address address, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO addresses (address_type_id,
                                                    city_id,
                                                    street, 
                                                    number,
                                                    complement,
                                                    neighborhood,
                                                    zip_code,
                                                    federal_unit)
                                            VALUES (@AddressTypeID,
                                                    @CityID,
                                                    @Street,
                                                    @Number,
                                                    @Complement,
                                                    @Neightborhood,
                                                    @ZipCode,
                                                    @FederalUnit)
                                         RETURNING id";

        var command = new CommandDefinition(sql, address, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<long>(command);
    }

    public async Task Insert(IEnumerable<Address> addressess, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO addresses (id,
                                                    address_type_id,
                                                    city_id,
                                                    street, 
                                                    number,
                                                    complement,
                                                    neighborhood,
                                                    zip_code,
                                                    federal_unit)
                                            VALUES (@ID,
                                                    @AddressTypeID,
                                                    @CityID,
                                                    @Street,
                                                    @Number,
                                                    @Complement,
                                                    @Neightborhood,
                                                    @ZipCode,
                                                    @FederalUnit)";

        var command = new CommandDefinition(sql, addressess, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task CopyToTable(IEnumerable<Address> addresses, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        using var writer = await npgsqlConn.BeginBinaryImportAsync(@"
        COPY addresses (id,
                        address_type_id,
                        city_id,
                        street, 
                        number,
                        complement,
                        neighborhood,
                        zip_code,
                        federal_unit)
        FROM STDIN (FORMAT BINARY)", cancellationToken);

        var rowCount = 0;
        foreach (var address in addresses)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();
                await writer.StartRowAsync(cancellationToken);

                await writer.WriteAsync(address.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(address.AddressTypeID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(address.CityID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(address.Street, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(address.Number ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(address.Complement ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(address.Neightborhood, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(address.ZipCode ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(address.FederalUnit, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing company at row {rowCount}, ID: {address.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
