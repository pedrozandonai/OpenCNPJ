namespace OpenCnpj.Application.LegalNatures.Domain;

public class LegalNature
{
    public long Id { get; protected set; }
    public string Code { get; init; }
    public string Description { get; private set; }

    private LegalNature(long iD, string code, string description)
    {
        Id = iD;
        Code = code;
        Description = description;
    }

    private LegalNature()
    {
    }

    public static LegalNature Create(string code, string description)
        => new(0, code, description);
}
