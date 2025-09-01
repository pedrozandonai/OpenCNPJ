using Dapper;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
public class LegalNatureRepository(IDatabaseFactory databaseFactory) : ILegalNatureRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;
    public async Task Insert(LegalNature legalNature, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO legal_natures (description)
                                            VALUES (@Description)";

        var command = new CommandDefinition(sql, legalNature, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }
}
