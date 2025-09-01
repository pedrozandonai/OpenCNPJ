using OpenCnpj.ConsoleApp.Application.AddressTypes.Domain;
using OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;
public class PartnerQualification : BaseRecord
{
    private PartnerQualification(int id, string description) : base(id, description)
    {
    }

    public static PartnerQualification Create(string description)
        => new(0, description);
}
