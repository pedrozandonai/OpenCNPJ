namespace OpenCnpj.Application.CompanySpecialSituations.Domain;

public class CompanySpecialSituation
{
    public long ID { get; set; }

    public long CompanyID { get; set; }

    public long SpecialSituationID { get; set; }

    public DateTime StartDate { get; set; }

    private CompanySpecialSituation(long id, long companyID, long specialSituationID, DateTime startDate)
    {
        ID = id;
        CompanyID = companyID;
        SpecialSituationID = specialSituationID;
        StartDate = startDate.ToLocalTime();
    }

    public static CompanySpecialSituation Create(long id, long companyID, long specialSituationID, DateTime startDate)
        => new(id, companyID, specialSituationID, startDate);
}
