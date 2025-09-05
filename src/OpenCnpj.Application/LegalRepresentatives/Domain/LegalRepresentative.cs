namespace OpenCnpj.Application.LegalRepresentatives.Domain;
public class LegalRepresentative
{
    public long Id { get; private set; }
    public int PartnerQualificationId { get; private set; }
    public string Identifier { get; private set; }
    public string Name { get; private set; }
}
