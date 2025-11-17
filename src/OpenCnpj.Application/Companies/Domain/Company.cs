namespace OpenCnpj.Application.Companies.Domain;

public class Company
{
    public long ID { get; private set; }

    public long LegalNatureID { get; private set; }

    public long? MainPartnerQualificationID { get; private set; }

    public int CompanySizeID { get; private set; }

    public int CompanyTypeID { get; private set; }

    public long? CountryID { get; private set; }

    public long AddressID { get; private set; }

    public long MainEconomicActivityID { get; private set; }

    public string Identifier { get; private set; }

    public string Name { get; private set; }

    public decimal ShareCapital { get; private set; }

    public string? ResponsableFederativeEntity { get; private set; }

    public string? FantasyName { get; private set; }

    public DateTime? RegisterDate { get; private set; }

    public string? ForeingCityName { get; private set; }

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
