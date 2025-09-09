using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.Companies.Domain;

[PgTable("companies")]
public class Company
{
    [PgColumn("id", 1)]
    public long ID { get; private set; }

    [PgColumn("legal_nature_id", 2)]
    public long LegalNatureID { get; private set; }

    [PgColumn("main_partner_qualification_id", 3)]
    public long? MainPartnerQualificationID { get; private set; }

    [PgColumn("company_size_id", 4)]
    public int CompanySizeID { get; private set; }

    [PgColumn("company_type_id", 5)]
    public int CompanyTypeID { get; private set; }

    [PgColumn("country_id", 6)]
    public long? CountryID { get; private set; }

    [PgColumn("address_id", 7)]
    public long AddressID { get; private set; }

    [PgColumn("main_economic_activity_id", 8)]
    public long MainEconomicActivityID { get; private set; }

    [PgColumn("identifier", 9)]
    public string Identifier { get; private set; }

    [PgColumn("name", 10)]
    public string Name { get; private set; }

    [PgColumn("share_capital", 11)]
    public decimal ShareCapital { get; private set; }

    [PgColumn("responsabile_federative_entity", 12)]
    public string? ResponsableFederativeEntity { get; private set; }

    [PgColumn("fantasy_name", 13)]
    public string? FantasyName { get; private set; }

    [PgColumn("register_date", 14)]
    public DateTime? RegisterDate { get; private set; }

    [PgColumn("foreign_city_name", 15)]
    public string? ForeingCityName { get; private set; }

    [PgColumn("start_date", 16)]
    public DateTime StartDate { get; private set; }

    private Company(long iD, long legalNatureID, long? mainPartnerQualificationID, int companySizeID, int companyTypeID, long? countryID, long addressID, long mainEconomicActivityID, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateTime? registerDate, string? foreingCityName, DateTime startDate)
    {
        ID = iD;
        LegalNatureID = legalNatureID;
        MainPartnerQualificationID = mainPartnerQualificationID;
        CompanySizeID = companySizeID;
        CompanyTypeID = companyTypeID;
        CountryID = countryID;
        AddressID = addressID;
        MainEconomicActivityID = mainEconomicActivityID;
        Identifier = identifier;
        Name = name;
        ShareCapital = shareCapital;
        ResponsableFederativeEntity = responsableFederativeEntity;
        FantasyName = fantasyName;
        RegisterDate = registerDate.HasValue ? registerDate.Value.ToLocalTime() : null;
        ForeingCityName = foreingCityName;
        StartDate = startDate.ToLocalTime();
    }

    public static Company Create(long id, long legalNatureID, long? mainPartnerQualificationID, int companySizeID, int companyTypeID, long? countryID, long addressID, long mainEconomicActivityID, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateTime? registerDate, string? foreingCityName, DateTime startDate)
        => new(id, legalNatureID, mainPartnerQualificationID, companySizeID, companyTypeID, countryID, addressID, mainEconomicActivityID, identifier, name, shareCapital, responsableFederativeEntity, fantasyName, registerDate, foreingCityName, startDate);
}
