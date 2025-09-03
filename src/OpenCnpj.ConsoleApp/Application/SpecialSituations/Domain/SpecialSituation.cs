using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;
public class SpecialSituation
{
    public long ID { get; private set; }
    public string Description { get; private set; }
    public DateOnly SituationDate { get; private set; }

    public SpecialSituation(long id, string description, DateOnly situationDate)
    {
        ID = id;
        Description = description;
        SituationDate = situationDate;
    }

    public static SpecialSituation Create(string description, DateOnly situationDate)
        => new SpecialSituation(0, description, situationDate);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'SpecialSituation' already has been set.");

        ID = id;

        return Result.Success();
    }
}
