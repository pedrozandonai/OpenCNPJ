using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;
using OpenCnpj.ConsoleApp.Application.Cities.Domain;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
public class AddressType : BaseRecord
{
    private AddressType(int id, string description) : base(id, description)
    {
    }

    public static AddressType Create(string description)
        => new(0, description);
}
