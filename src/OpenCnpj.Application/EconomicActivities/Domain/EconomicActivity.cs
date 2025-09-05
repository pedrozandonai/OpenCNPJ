namespace OpenCnpj.Application.EconomicActivities.Domain;

public class EconomicActivity
{
    public long Id { get; init; }
    public string Code { get; init; }
    public string Description { get; private set; }

    public EconomicActivity(long id, string code, string description)
    {
        Id = id;
        Code = code;
        Description = description;
    }

    public static EconomicActivity Create(string code, string description)
        => new(0, code, description);
}
