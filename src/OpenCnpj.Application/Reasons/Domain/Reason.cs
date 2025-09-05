using OpenCnpj.Application.BaseRecords.Abstractions;

namespace OpenCnpj.Application.Reasons.Domain;
public class Reason : BaseRecord
{
    public string Code { get; init; }

    private Reason(int id, string code, string description)
        : base(id, description)
    {
        Code = code;
    }

    public static Reason Create(string code, string description)
        => new(0, code, description);
}
