using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.AddressTypes.Domain;

public class AddressType
{
    public long ID { get; private set; }

    public string Description { get; private set; }

    private AddressType(long id, string description)
    {
        ID = id;
        Description = description;
    }

    public static AddressType Create(long id, string description)
        => new(id, description);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'AddressType' already has been set.");

        ID = id;

        return Result.Success();
    }
}
