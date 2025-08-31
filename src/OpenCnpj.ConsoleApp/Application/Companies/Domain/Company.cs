namespace OpenCnpj.ConsoleApp.Application.Companies.Domain;
public class Company
{
    public long ID { get; private set; }
    public int LegalNatureID { get; private set; }
    public int MainPartnerQualificationID { get; private set; }
    public int CompanySizeID { get; private set; }
    public int CompanyTypeID { get; private set; }
    public int ReasonID { get; private set; }
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
}
