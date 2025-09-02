namespace OpenCnpj.ConsoleApp.Application.LegalRepresentatives.Domain;
public class LegalRepresentative
{
    public long ID { get; private set; }
    public int PartnerQualificationID { get; private set; }
    public string Identifier { get; private set; }
    public string Name { get; private set; }
}
