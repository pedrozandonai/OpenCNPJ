using Dapper;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Repositories;
public class AddressTypeRepository(IDatabaseFactory databaseFactory) : IAddressTypeRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<int> Insert(AddressType addressType, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO address_types (description)
                                                VALUES (@Description)
                                             RETURNING id";

        var command = new CommandDefinition(sql, addressType, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<int>(command);
    }


    public async Task<AddressType?> GetByDescription(string description, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    description AS Description
                               FROM address_types
                              WHERE description = @description";

        var command = new CommandDefinition(sql, new { description }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<AddressType>(command);
    }
}
