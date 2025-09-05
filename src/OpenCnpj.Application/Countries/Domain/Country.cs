namespace OpenCnpj.Application.Countries.Domain;

public class Country
{
    public long Id { get; protected set; }
    public string Code { get; init; }
    public string Description { get; private set; }

    private Country(long iD, string code, string description)
    {
        Id = iD;
        Code = code;
        Description = description;
    }

    private Country()
    {
    }

    public static Country Create(string code, string description)
        => new(0, code, description);
}
