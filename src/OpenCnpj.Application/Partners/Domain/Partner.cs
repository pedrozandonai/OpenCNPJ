namespace OpenCnpj.Application.Partners.Domain;
public class Partner
{
    public long ID { get; private set; }
    public long CompanyID { get; private set; }
    public long? CountryID { get; private set; }
    public int PartnerTypeID { get; private set; }
    public long? LegalRepresentativeID { get; private set; }
    public long QualificationID { get; private set; }
    public int AgeRangeID { get; private set; }
    public string Name { get; private set; }
    public string? Identifier { get; private set; }
    public DateTime StartDate { get; private set; }

    private Partner(long iD, long companyID, int partnerTypeID, long? legalRepresentativeID, long qualificationID, string name, string? identifier, DateTime startDate, long? countryID, int ageRangeID)
    {
        ID = iD;
        CompanyID = companyID;
        PartnerTypeID = partnerTypeID;
        LegalRepresentativeID = legalRepresentativeID;
        QualificationID = qualificationID;
        Name = name;
        Identifier = identifier;
        StartDate = startDate.ToLocalTime();
        CountryID = countryID;
        AgeRangeID = ageRangeID;
    }

    public static Partner Create(long id, long companyID, int partnerTypeID, long? legalRepresentativeID, long qualificationID, string name, string? identifier, DateTime startDate, long? countryID, int ageRangeID)
        => new(id, companyID, partnerTypeID, legalRepresentativeID, qualificationID, name, identifier, startDate, countryID, ageRangeID);
}
