namespace OpenCnpj.ConsoleApp.Application.Companies.Domain;
public class Company
{
    public long ID { get; private set; }
    public int LegalNatureID { get; private set; }
    public int MainPartnerQualificationID { get; private set; }
    public int CompanySizeID { get; private set; }
    public int CompanyTypeID { get; private set; }
    public int CountryID { get; private set; }
    public long AddressID { get; private set; }
    public long? SpecialSituationID { get; private set; }
    public long MainEconomicActivityID { get; private set; }
    public string Identifier { get; private set; }
    public string Name { get; private set; }
    public decimal ShareCapital { get; private set; }
    public string? ResponsableFederativeEntity { get; private set; }
    public string? FantasyName { get; private set; }
    public DateOnly? RegisterDate { get; private set; }
    public string? ForeingCityName { get; private set; }
    public DateOnly StartDate { get; private set; }

    private Company(long iD, int legalNatureID, int mainPartnerQualificationID, int companySizeID, int companyTypeID, int countryID, long addressID, long? specialSituationID, long mainEconomicActivityID, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateOnly? registerDate, string? foreingCityName, DateOnly startDate)
    {
        ID=iD;
        LegalNatureID=legalNatureID;
        MainPartnerQualificationID=mainPartnerQualificationID;
        CompanySizeID=companySizeID;
        CompanyTypeID=companyTypeID;
        CountryID=countryID;
        AddressID=addressID;
        SpecialSituationID=specialSituationID;
        MainEconomicActivityID=mainEconomicActivityID;
        Identifier=identifier;
        Name=name;
        ShareCapital=shareCapital;
        ResponsableFederativeEntity=responsableFederativeEntity;
        FantasyName=fantasyName;
        RegisterDate=registerDate;
        ForeingCityName=foreingCityName;
        StartDate=startDate;
    }

    public static Company Create(int legalNatureID, int mainPartnerQualificationID, int companySizeID, int companyTypeID, int countryID, long addressID, long? specialSituationID, long mainEconomicActivityID, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateOnly? registerDate, string? foreingCityName, DateOnly startDate)
        => new(0, legalNatureID, mainPartnerQualificationID, companySizeID, companyTypeID, countryID, addressID, specialSituationID, mainEconomicActivityID, identifier, name, shareCapital, responsableFederativeEntity, fantasyName, registerDate, foreingCityName, startDate);
}
