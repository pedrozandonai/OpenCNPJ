using CSharpFunctionalExtensions;
using OpenCnpj.Application.SpecialSituations.Domain;

namespace OpenCnpj.Application.CompanySpecialSituations.Domain;
public class CompanySpecialSituation
{
    public long Id { get; set; }
    public SpecialSituation SpecialSituation { get; set; }
    public DateTime StartDate { get; set; }

    private CompanySpecialSituation(long id, SpecialSituation specialSituation, DateTime startDate)
    {
        Id = id;
        SpecialSituation = specialSituation;
        StartDate = startDate;
    }

    private CompanySpecialSituation()
    {
    }

    public static CompanySpecialSituation Create(long id, SpecialSituation specialSituation, DateTime startDate)
        => new(id, specialSituation, startDate);

    public Result SetId(int id)
    {
        if (Id != 0)
            return Result.Failure("The Id for the record 'CompanySpecialSituation' already has been set.");

        Id = id;

        return Result.Success();
    }
}
