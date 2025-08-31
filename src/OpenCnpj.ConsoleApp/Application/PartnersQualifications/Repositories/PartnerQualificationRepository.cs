using Dapper;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
public class PartnerQualificationRepository(IDatabaseFactory databaseFactory) : IPartnerQualificationRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;
    public async Task Insert(PartnerQualification partnerQualification, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO partner_qualifications (description)
                                                         VALUES (@Description)";

        var command = new CommandDefinition(sql, partnerQualification, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }
}
