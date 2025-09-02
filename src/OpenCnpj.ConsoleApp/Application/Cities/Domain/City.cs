using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.Cities.Domain;
public class City : BaseRecord
{
    public string Code { get; init; }

    private City(int id, string code, string description)
        : base(id, description)
    {
        Code = code;
    }

    private City()
        : base()
    {
    }

    public static City Create(string code, string description)
        => new (0, code, description);
}
