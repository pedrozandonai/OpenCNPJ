using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
public class LegalNature : BaseRecord
{
    public string Code { get; init; }

    private LegalNature(int id, string code, string description)
        : base(id, description)
    {
        Code = code;
    }

    public static LegalNature Create(string code, string description)
        => new(0, code, description);
}
