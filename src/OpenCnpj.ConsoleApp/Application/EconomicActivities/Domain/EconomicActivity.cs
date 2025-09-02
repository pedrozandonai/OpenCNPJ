using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;
public class EconomicActivity : BaseRecord
{
    public string Code { get; init; }

    private EconomicActivity(int id, string code, string description) : base(id, description)
    {
        Code = code;
    }

    public static EconomicActivity Create(string code, string description)
        => new(0, code, description);
}
