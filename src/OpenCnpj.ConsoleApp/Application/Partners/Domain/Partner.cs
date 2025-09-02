namespace OpenCnpj.ConsoleApp.Application.Partners.Domain;
public class Partner
{
    public long ID { get; private set; }
    public long CompanyID { get; private set; }
    public int PartnerTypeID { get; private set; }
    public long? LegalRepresentativeID { get; private set; }
    public int PartnerQualificationID { get; private set; }
    public string Name { get; private set; }
    public string? Identifier { get; private set; }
    public DateOnly StartDate { get; private set; }
    public int? CountryID { get; private set; }
    public int AgeRangeID { get; private set; }

    private Partner(long iD, long companyID, int partnerTypeID, long? legalRepresentativeID, int partnerQualificationID, string name, string? identifier, DateOnly startDate, int? countryID, int ageRangeID)
    {
        ID=iD;
        CompanyID=companyID;
        PartnerTypeID=partnerTypeID;
        LegalRepresentativeID=legalRepresentativeID;
        PartnerQualificationID=partnerQualificationID;
        Name=name;
        Identifier=identifier;
        StartDate=startDate;
        CountryID=countryID;
        AgeRangeID=ageRangeID;
    }

    public static Partner Create(long companyID, int partnerTypeID, long? legalRepresentativeID, int partnerQualificationID, string name, string? identifier, DateOnly startDate, int? countryID, int ageRangeID)
        => new (0, companyID, partnerTypeID, legalRepresentativeID, partnerQualificationID, name, identifier, startDate, countryID, ageRangeID);
}
