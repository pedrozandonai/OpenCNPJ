using Dapper;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.LegalNatures.Repositories;

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
                               FROM legal_natures
                              WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<LegalNature>(command);
    }

    public async Task<IEnumerable<LegalNature>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM legal_natures";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<LegalNature>(command);
    }
}
