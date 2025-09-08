namespace OpenCnpj.Application.LegalRepresentatives.Domain;
public class LegalRepresentative
{
    public long ID { get; private set; }
    public long QualificationID { get; private set; }
    public string Identifier { get; private set; }
    public string Name { get; private set; }

    private LegalRepresentative(long id, long qualificationID, string identifier, string name)
    {
        ID = id;
        QualificationID = qualificationID;
        Identifier = identifier;
        Name = name;
    }

    public static LegalRepresentative Create(long id, long qualificationID, string identifier, string name)
        => new(id, qualificationID, identifier, name);
}
