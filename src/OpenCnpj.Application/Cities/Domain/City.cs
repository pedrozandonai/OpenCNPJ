namespace OpenCnpj.Application.Cities.Domain;
public class City
{
    public long Id { get; protected set; }
    public long Code { get; init; }
    public string Description { get; private set; }

    private City(long id, long code, string description)
    {
        Id = id;
        Code = code;
        Description = description;
    }

    private City()
    {
    }

    public static City Create(long code, string description)
        => new (0, code, description);
}
