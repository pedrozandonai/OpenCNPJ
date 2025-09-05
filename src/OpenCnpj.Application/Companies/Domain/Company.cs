using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.Enums;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.PartnersQualifications.Domain;

namespace OpenCnpj.Application.Companies.Domain;
public class Company
{
    public long Id { get; private set; }
    public LegalNature LegalNature { get; private set; }
    public PartnerQualification? MainPartnerQualification { get; private set; }
    public ECompanySize CompanySize { get; private set; }
    public ECompanyType CompanyType { get; private set; }
    public Country? Country { get; private set; }
    public Address Address { get; private set; }
    public CompanySpecialSituation? CompanySpecialSituation { get; private set; }
    public EconomicActivity MainEconomicActivity { get; private set; }
    public string Identifier { get; private set; }
    public string Name { get; private set; }
    public decimal ShareCapital { get; private set; }
    public string? ResponsableFederativeEntity { get; private set; }
    public string? FantasyName { get; private set; }
    public DateTime? RegisterDate { get; private set; }
    public string? ForeingCityName { get; private set; }
    public DateTime StartDate { get; private set; }

    private Company(long id, LegalNature legalNature, PartnerQualification? mainPartnerQualification, ECompanySize companySize, ECompanyType companyType, Country? country, Address address, CompanySpecialSituation? companySpecialSituation, EconomicActivity mainEconomicActivity, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateTime? registerDate, string? foreingCityName, DateTime startDate)
    {
        Id = id;
        LegalNature = legalNature;
        MainPartnerQualification = mainPartnerQualification;
        CompanySize = companySize;
        CompanyType = companyType;
        Country = country;
        Address = address;
        CompanySpecialSituation = companySpecialSituation;
        MainEconomicActivity = mainEconomicActivity;
        Identifier = identifier;
        Name = name;
        ShareCapital = shareCapital;
        ResponsableFederativeEntity = responsableFederativeEntity;
        FantasyName = fantasyName;
        RegisterDate = registerDate;
        ForeingCityName = foreingCityName;
        StartDate = startDate;
    }

    private Company()
    {
    }

    public static Company Create(long id, LegalNature legalNature, PartnerQualification? mainPartnerQualification, ECompanySize companySize, ECompanyType companyType, Country? country, Address address, CompanySpecialSituation? companySpecialSituation, EconomicActivity mainEconomicActivity, string identifier, string name, decimal shareCapital, string? responsableFederativeEntity, string? fantasyName, DateTime? registerDate, string? foreingCityName, DateTime startDate)
        => new(id, legalNature, mainPartnerQualification, companySize, companyType, country, address, companySpecialSituation, mainEconomicActivity, identifier, name, shareCapital, responsableFederativeEntity, fantasyName, registerDate, foreingCityName, startDate);
}
