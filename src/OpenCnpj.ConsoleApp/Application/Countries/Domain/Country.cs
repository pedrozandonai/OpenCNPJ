using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.Countries.Domain;
public class Country : BaseRecord
{
    private Country(int id, string description) : base(id, description)
    {
    }

    public static Country Create(string description)
        => new(0, description);
}
