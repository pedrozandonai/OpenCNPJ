namespace OpenCnpj.Application.Simples.Domain;
public class Simple
{
    public long Id { get; private set; }
    public int CompanyId { get; private set; }
    public long? MeiId { get; private set; }
    public bool? IsSimple { get; private set; }
    public DateOnly? DateOpted { get; private set; }
    public DateOnly? ExclusionDate { get; private set; }
}
