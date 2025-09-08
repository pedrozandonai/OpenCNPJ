namespace OpenCnpj.Application.Meis.Domain;
public class Mei
{
    public long ID { get; private set; }
    public DateTime? DateOpted { get; private set; }
    public DateTime? ExclusionDate { get; private set; }

    private Mei(long id, DateTime? dateOpted, DateTime? exclusionDate)
    {
        ID = id;
        DateOpted = dateOpted.HasValue ? dateOpted.Value.ToLocalTime() : null;
        ExclusionDate = exclusionDate.HasValue ? exclusionDate.Value.ToLocalTime() : null;
    }

    public static Mei Create(long id, DateTime? dateOpted, DateTime? exclusionDate)
        => new(id, dateOpted, exclusionDate);
}
