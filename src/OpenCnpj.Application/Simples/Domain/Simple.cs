namespace OpenCnpj.Application.Simples.Domain;
public class Simple
{
    public long ID { get; private set; }
    public long CompanyID { get; private set; }
    public long? MeiID { get; private set; }
    public bool? IsSimple { get; private set; }
    public DateTime? DateOpted { get; private set; }
    public DateTime? ExclusionDate { get; private set; }

    private Simple(long id, long companyID, long? meiID, bool? isSimple, DateTime? dateOpted, DateTime? exclusionDate)
    {
        ID = id;
        CompanyID = companyID;
        MeiID = meiID;
        IsSimple = isSimple;
        DateOpted = dateOpted.HasValue ? dateOpted.Value.ToLocalTime() : null;
        ExclusionDate = exclusionDate.HasValue ? exclusionDate.Value.ToLocalTime() : null;
    }

    public static Simple Create(long id, long companyID, long? meiID, bool? isSimple, DateTime? dateOpted, DateTime? exclusionDate)
        => new(id, companyID, meiID, isSimple, dateOpted, exclusionDate);
}
