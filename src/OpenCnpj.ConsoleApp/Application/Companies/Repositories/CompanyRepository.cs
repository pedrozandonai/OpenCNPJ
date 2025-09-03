using Dapper;
using OpenCnpj.ConsoleApp.Application.Companies.Domain;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Application.Companies.Repositories;
public class CompanyRepository(IDatabaseFactory databaseFactory) : ICompanyRepository
{
    public IDatabaseFactory DatabaseFactory => databaseFactory;

    public async Task Insert(IEnumerable<Company> companies, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO company (legal_nature_id,
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
                                          VALUES (@LegalNatureID,
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
}
