using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.CompanySpecialSituations.Domain;

[PgTable("company_special_situations")]
public class CompanySpecialSituation
{
    [PgColumn("id", 1)]
    public long ID { get; set; }

    [PgColumn("company_id", 2)]
    public long CompanyID { get; set; }

    [PgColumn("special_situation_id", 3)]
    public long SpecialSituationID { get; set; }

    [PgColumn("start_date", 4)]
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
