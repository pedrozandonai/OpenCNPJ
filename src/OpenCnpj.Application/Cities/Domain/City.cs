namespace OpenCnpj.Application.Cities.Domain;
public class City
{
    public long ID { get; protected set; }
    public long Code { get; init; }
    public string Description { get; private set; }

    public City(long iD, long code, string description)
    {
        ID = iD;
        Code = code;
        Description = description;
    }

    public static City Create(long code, string description)
        => new (0, code, description);
}
