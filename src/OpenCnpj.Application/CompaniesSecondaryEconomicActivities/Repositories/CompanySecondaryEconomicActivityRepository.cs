using Npgsql;
using OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Repositories;
public class CompanySecondaryEconomicActivityRepository(IDatabaseFactory databaseFactory) : ICompanySecondaryEconomicActivityRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task CopyToTable(IEnumerable<CompanySecondaryEconomicActivity> companySecondaryEconomicActivities, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        const string sql = @"COPY company_secondary_economic_activities (company_id,
                                                                         economic_activity_id)
                                                             FROM STDIN (FORMAT BINARY)";

        using var writer = await npgsqlConn.BeginBinaryImportAsync(sql, cancellationToken);

        var rowCount = 0;
        foreach (var companySecondaryEconomicActivity in companySecondaryEconomicActivities)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(companySecondaryEconomicActivity.CompanyID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(companySecondaryEconomicActivity.EconomicActivityID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing companySecondaryEconomicActivity at row {rowCount}, CompanyID: {companySecondaryEconomicActivity.CompanyID}, EconomicActivityID: {companySecondaryEconomicActivity.EconomicActivityID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
