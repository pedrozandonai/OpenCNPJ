namespace OpenCnpj.Application.IndividualMicroentrepreneurs.Domain;
public class IndividualMicroentrepreneur
{
    public long Id { get; private set; }
    public DateOnly? DateOpted { get; private set; }
    public DateOnly? ExclusionDate { get; private set; }
}
