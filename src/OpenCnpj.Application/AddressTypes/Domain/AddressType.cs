using CSharpFunctionalExtensions;
using OpenCnpj.Application.BaseRecords.Abstractions;

namespace OpenCnpj.Application.AddressTypes.Domain;
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

    public static AddressType Create(int id, string description)
        => new(id, description);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'AddressType' already has been set.");

        ID = id;

        return Result.Success();
    }
}
