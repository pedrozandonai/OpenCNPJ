namespace OpenCnpj.ConsoleApp.Application.LegalNatures.Domain;

public class LegalNature
{
    public long ID { get; protected set; }
    public string Code { get; init; }
    public string Description { get; private set; }

    private LegalNature(long iD, string code, string description)
    {
        ID = iD;
        Code = code;
        Description = description;
    }

    public static LegalNature Create(string code, string description)
        => new(0, code, description);
}
