using Dapper;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
public class PartnerQualificationRepository(IDatabaseFactory databaseFactory) : IPartnerQualificationRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;
    public async Task Insert(PartnerQualification partnerQualification, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO partner_qualifications (code,
                                                                 description)
                                                         VALUES (@Code,
                                                                 @Description)";

        var command = new CommandDefinition(sql, partnerQualification, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task<PartnerQualification?> GetByCode(long code, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM partner_qualifications
                              WHERE code = @code";

        var command = new CommandDefinition(sql, new { code }, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryFirstOrDefaultAsync<PartnerQualification>(command);
    }

    public async Task<IEnumerable<PartnerQualification>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS ID,
                                    code AS Code,
                                    description AS Description
                               FROM partner_qualifications";

        var command = new CommandDefinition(sql, transaction: DatabaseFactory.Transaction, cancellationToken:cancellationToken);

        return await DatabaseFactory.Connection.QueryAsync<PartnerQualification>(command);
    }
}
