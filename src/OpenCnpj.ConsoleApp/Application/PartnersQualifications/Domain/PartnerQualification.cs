using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
public class PartnerQualification : BaseRecord
{
    public string Code { get; init; }

    private PartnerQualification(int id, string code, string description)
        : base(id, description)
    {
        Code = code;
    }

    public static PartnerQualification Create(string code, string description)
        => new(0, code, description);
}
