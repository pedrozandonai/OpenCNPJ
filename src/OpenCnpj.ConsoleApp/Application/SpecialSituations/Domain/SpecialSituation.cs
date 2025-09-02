namespace OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
public class SpecialSituation
{
    public long ID { get; private set; }
    public string Description { get; private set; }
    public DateOnly SituationDate { get; private set; }
}
