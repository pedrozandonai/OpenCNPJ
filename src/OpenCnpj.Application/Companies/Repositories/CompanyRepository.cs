using Dapper;
using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Database.Services;

namespace OpenCnpj.Application.Companies.Repositories;
public class CompanyRepository(IDatabaseFactory databaseFactory, IPgBulkCopyService bulk, TweakSettings tweakSettings) : ICompanyRepository
{
    public async Task CopyToTable(IEnumerable<Company> companies, CancellationToken cancellationToken)
        => await bulk.CopyAsync(databaseFactory.ConnectionString, companies, tweakSettings.FormatRawDataSettings.RecordsBatchAmount, cancellationToken);

    public async Task<Company?> GetByBasicCnpj(string basicCnpj, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT id AS id,
                                    legal_nature_id AS legalnatureid,
                                    main_partner_qualification_id AS mainpartnerqualificationid,
                                    company_size_id AS companysizeid,
                                    company_type_id AS companytypeid,
                                    country_id AS countryid,
                                    address_id AS addressid,
                                    company_special_situation_id AS companyspecialsituationid,
                                    main_economic_activity_id AS maineconomicactivityid,
                                    identifier AS identifier,
                                    name AS name,
                                    share_capital AS sharecapital,
                                    responsable_federative_entity AS responsablefederativeentity,
                                    fantasy_name AS fantasyname,
                                    register_date AS registerdate,
                                    foreing_city_name AS foreingcityname,
                                    start_date AS startdate
                               FROM companies
                              WHERE identifier LIKE @pattern";

        var command = new CommandDefinition(sql, new { pattern = basicCnpj + "%" }, transaction: databaseFactory.Transaction, cancellationToken: cancellationToken);

        return await databaseFactory.Connection.QueryFirstOrDefaultAsync<Company>(command);
    }
}
