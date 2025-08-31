using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.Reasons.Domain;
public class Reason : BaseRecord
{
    private Reason(int id, string description) : base(id, description)
    {
    }

    public static Reason Create(string description)
        => new(0, description);
}
