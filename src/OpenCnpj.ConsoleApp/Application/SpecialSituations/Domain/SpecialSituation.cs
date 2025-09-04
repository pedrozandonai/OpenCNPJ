using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
public class SpecialSituation
{
    public int ID { get; private set; }
    public string Description { get; private set; }

    public SpecialSituation(int id, string description)
    {
        ID = id;
        Description = description;
    }

    public static SpecialSituation Create(int id, string description)
        => new (id, description);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'SpecialSituation' already has been set.");

        ID = id;

        return Result.Success();
    }
}
