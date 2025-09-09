using Dapper;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Database.Services;

namespace OpenCnpj.Application.CompanySpecialSituations.Repositories;
public class CompanySpecialSituationRepository(IDatabaseFactory databaseFactory, IPgBulkCopyService bulk, TweakSettings tweakSettings) : ICompanySpecialSituationRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task<int> Insert(CompanySpecialSituation companySpecialSituation, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO company_special_situations (special_situation_id,
                                                                     start_date)
                                                             VALUES (@SpecialSituationID,
                                                                     @StartDate)
                                                          RETURNING id";

        var command = new CommandDefinition(sql, companySpecialSituation, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        return await DatabaseFactory.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task Insert(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO company_special_situations (id,
                                                                     special_situation_id,
                                                                     start_date)
                                                             VALUES (@ID,
                                                                     @SpecialSituationID,
                                                                     @StartDate)";

        var command = new CommandDefinition(sql, companySpecialSituations, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task CopyToTable(IEnumerable<CompanySpecialSituation> companySpecialSituations, CancellationToken cancellationToken)
        => await bulk.CopyAsync(databaseFactory.ConnectionString, companySpecialSituations, tweakSettings.FormatRawDataSettings.RecordsBatchAmount, cancellationToken);
}
