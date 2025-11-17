namespace OpenCnpj.Application.SpecialSituations.Domain;

public class SpecialSituation
{
    public long ID { get; private set; }

    public string Description { get; private set; }

    public SpecialSituation(long id, string description)
    {
        ID = id;
        Description = description;
    }

    public static SpecialSituation Create(long id, string description)
        => new (id, description);
}
