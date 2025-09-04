using Dapper;
using Npgsql;
using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Companies.Repositories;
public class CompanyRepository(IDatabaseFactory databaseFactory) : ICompanyRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(IEnumerable<Company> companies, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO company (id,
                                                  legal_nature_id,
                                                  main_partner_qualification_id,
                                                  company_size_id,
                                                  company_type_id,
                                                  country_id,
                                                  address_id,
                                                  company_special_situation_id,
                                                  main_economic_activity_id,
                                                  identifier, 
                                                  name,
                                                  share_capital,
                                                  responsabile_federative_entity,
                                                  fantasy_name,
                                                  register_date,
                                                  foreign_city_name,
                                                  start_date)
                                          VALUES (@ID,
                                                  @LegalNatureID,
                                                  @MainPartnerQualificationID,
                                                  @CompanySizeID,
                                                  @CompanyTypeID,
                                                  @CountryID,
                                                  @AddressID,
                                                  @CompanySpecialSituationID,
                                                  @MainEconomicActivityID,
                                                  @Identifier,
                                                  @Name,
                                                  @ShareCapital,
                                                  @ResponsableFederativeEntity,
                                                  @FantasyName,
                                                  @RegisterDate,
                                                  @ForeingCityName,
                                                  @StartDate)";

        var command = new CommandDefinition(sql, companies, transaction: DatabaseFactory.Transaction, cancellationToken: cancellationToken);

        await DatabaseFactory.Connection.ExecuteAsync(command);
    }

    public async Task CopyToTable(IEnumerable<Company> companies, CancellationToken cancellationToken)
    {
        if (DatabaseFactory.Connection is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Database connection must be NpgsqlConnection for COPY.");

        using var writer = await npgsqlConn.BeginBinaryImportAsync(@"COPY company (id,
                                                                                   legal_nature_id,
                                                                                   main_partner_qualification_id,
                                                                                   company_size_id,
                                                                                   company_type_id,
                                                                                   country_id,
                                                                                   address_id,
                                                                                   company_special_situation_id,
                                                                                   main_economic_activity_id,
                                                                                   identifier,
                                                                                   name,
                                                                                   share_capital,
                                                                                   responsabile_federative_entity,
                                                                                   fantasy_name,
                                                                                   register_date,
                                                                                   foreign_city_name,
                                                                                   start_date)
                                                                       FROM STDIN (FORMAT BINARY)", cancellationToken);

        var rowCount = 0;
        foreach (var company in companies)
        {
            try
            {
                rowCount++;
                cancellationToken.ThrowIfCancellationRequested();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(company.ID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(company.LegalNatureID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(company.MainPartnerQualificationID ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(company.CompanySizeID, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(company.CompanyTypeID, NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(company.CountryID ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(company.AddressID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                int? companySpecialSituationId = company.CompanySpecialSituationID.HasValue ? (int)company.CompanySpecialSituationID.Value : null;
                await writer.WriteAsync(companySpecialSituationId ?? (object)DBNull.Value,NpgsqlTypes.NpgsqlDbType.Integer, cancellationToken);
                await writer.WriteAsync(company.MainEconomicActivityID, NpgsqlTypes.NpgsqlDbType.Bigint, cancellationToken);
                await writer.WriteAsync(company.Identifier, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(company.Name, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(company.ShareCapital, NpgsqlTypes.NpgsqlDbType.Numeric, cancellationToken);
                await writer.WriteAsync(company.ResponsableFederativeEntity ?? (object)DBNull.Value, NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(company.FantasyName ?? (object)DBNull.Value,NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(company.RegisterDate ?? (object)DBNull.Value,NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
                await writer.WriteAsync(company.ForeingCityName ?? (object)DBNull.Value,NpgsqlTypes.NpgsqlDbType.Text, cancellationToken);
                await writer.WriteAsync(company.StartDate, NpgsqlTypes.NpgsqlDbType.Timestamp, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing company at row {rowCount}, ID: {company.ID}", ex);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }
}
