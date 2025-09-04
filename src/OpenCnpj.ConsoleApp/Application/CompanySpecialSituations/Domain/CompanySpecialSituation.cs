using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.SpecialSituations.Domain;

namespace OpenCnpj.ConsoleApp.Application.CompanySpecialSituations.Domain;
public class CompanySpecialSituation
{
    public int ID { get; set; }
    public int SpecialSituationID { get; set; }
    public DateTime StartDate { get; set; }

    public CompanySpecialSituation(int iD, int specialSituationID, DateTime startDate)
    {
        ID = iD;
        SpecialSituationID = specialSituationID;
        StartDate = startDate;
    }

    public static CompanySpecialSituation Create(int id, int specialSituationID, DateTime startDate)
    => new(id, specialSituationID, startDate);

    public Result SetID(int id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'CompanySpecialSituation' already has been set.");

        ID = id;

        return Result.Success();
    }
}
