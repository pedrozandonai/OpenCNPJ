using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
public class AddressType : BaseRecord
{
    private AddressType(int id, string description)
        : base(id, description)
    {
    }

    private AddressType()
        : base()
    {
    }

    public static AddressType Create(string description)
        => new(0, description);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'AddressType' already has been set.");

        ID = id;

        return Result.Success();
    }
}
