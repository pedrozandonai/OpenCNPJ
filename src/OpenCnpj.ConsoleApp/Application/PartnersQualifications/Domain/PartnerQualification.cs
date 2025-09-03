namespace OpenCnpj.ConsoleApp.Application.PartnersQualifications.Domain;

public class PartnerQualification
{
    public long ID { get; init; }
    public long Code { get; init; }
    public string Description { get; private set; }

    private PartnerQualification(long id, long code, string description)
    {
        ID = id;
        Code = code;
        Description = description;
    }

    public static PartnerQualification Create(long code, string description)
        => new(0, code, description);
}
