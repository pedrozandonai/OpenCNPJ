namespace OpenCnpj.Application.Phones.Domain;
public class Phone
{
    public long Id { get; private set; }
    public int? AreaCode { get; private set; }
    public int? PhoneNumber { get; private set; }
}
