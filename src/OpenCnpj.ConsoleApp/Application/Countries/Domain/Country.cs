using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.Countries.Domain;
public class Country : BaseRecord
{
    public string Code { get; init; }

    private Country(int id, string code, string description) : base(id, description)
    {
        Code = code;
    }

    public static Country Create(string code, string description)
        => new(0, code, description);
}
