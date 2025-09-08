namespace OpenCnpj.Application.CompaniesSecondaryEconomicActivities.Domain;
public class CompanySecondaryEconomicActivity
{
    public long CompanyID { get; private set; }
    public long EconomicActivityID { get; private set; }

    private CompanySecondaryEconomicActivity(long companyID, long economicActivityID)
    {
        CompanyID = companyID;
        EconomicActivityID = economicActivityID;
    }

    public static CompanySecondaryEconomicActivity Create(long companyID, long economicActivityID)
        => new(companyID, economicActivityID);
}
