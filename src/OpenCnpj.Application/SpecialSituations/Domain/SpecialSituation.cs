using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.SpecialSituations.Domain;
public class SpecialSituation
{
    public int Id { get; private set; }
    public string Description { get; private set; }

    public SpecialSituation(int id, string description)
    {
        Id = id;
        Description = description;
    }

    public static SpecialSituation Create(int id, string description)
        => new (id, description);

    public Result SetId(int id)
    {
        if (Id != 0)
            return Result.Failure("The Id for the record 'SpecialSituation' already has been set.");

        Id = id;

        return Result.Success();
    }
}
