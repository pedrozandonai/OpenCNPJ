using Dapper;
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
}
