namespace OpenCnpj.ConsoleApp.Application.Simples.Domain;
public class Simple
{
    public long ID { get; private set; }
    public int CompanyID { get; private set; }
    public long? MeiID { get; private set; }
    public bool? IsSimple { get; private set; }
    public DateOnly? DateOpted { get; private set; }
    public DateOnly? ExclusionDate { get; private set; }
}
