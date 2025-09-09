using Dapper;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Database.Services;

namespace OpenCnpj.Application.Addresses.Repositories;
public class AddressRepository(IDatabaseFactory databaseFactory, IPgBulkCopyService bulk, TweakSettings tweakSettings) : IAddressRepository
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
        => await bulk.CopyAsync(databaseFactory.ConnectionString, addresses, tweakSettings.FormatRawDataSettings.RecordsBatchAmount, cancellationToken);
}
