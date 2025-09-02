using Dapper;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;

public class LegalNatureRepository(IDatabaseFactory databaseFactory) : ILegalNatureRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(LegalNature legalNature, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO legal_natures (code,
                                                        description)
                                                VALUES (@Code,
                                                        @Description)";

        var command = new CommandDefinition(sql, legalNature, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<LegalNature?> GetByCode(string code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM cities
                              WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<LegalNature>(command);
    }
}
