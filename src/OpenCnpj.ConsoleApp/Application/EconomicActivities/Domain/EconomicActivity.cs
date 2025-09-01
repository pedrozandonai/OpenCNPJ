using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.EconomicActivities.Domain;
public class EconomicActivity : BaseRecord
{
    private EconomicActivity(int id, string description) : base(id, description)
    {
    }

    public static EconomicActivity Create(string description)
        => new(0, description);
}
