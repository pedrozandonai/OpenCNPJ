using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;
public class LegalNature : BaseRecord
{
    private LegalNature(int id, string description) : base(id, description)
    {
    }

    public static LegalNature Create(string description)
        => new(0, description);
}
