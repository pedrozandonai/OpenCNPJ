namespace OpenCnpj.Application.Countries.Domain;

public class Country
{
    public long ID { get; protected set; }
    public string Code { get; init; }
    public string Description { get; private set; }

    public Country(long iD, string code, string description)
    {
        ID = iD;
        Code = code;
        Description = description;
    }

    public static Country Create(string code, string description)
        => new(0, code, description);
}
