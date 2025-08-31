using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.Cities.Domain;
public class City : BaseRecord
{
    private City(int id, string description)
        : base(id, description)
    {
    }

    public static City Create(string description)
        => new (0, description);
}
