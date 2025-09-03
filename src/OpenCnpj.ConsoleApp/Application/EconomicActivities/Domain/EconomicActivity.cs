namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;

public class EconomicActivity
{
    public long ID { get; init; }
    public string Code { get; init; }
    public string Description { get; private set; }

    public EconomicActivity(long id, string code, string description)
    {
        ID = id;
        Code = code;
        Description = description;
    }

    public static EconomicActivity Create(string code, string description)
        => new(0, code, description);
}
