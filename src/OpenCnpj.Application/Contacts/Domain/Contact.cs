namespace OpenCnpj.Application.Contacts.Domain;
public class Contact
{
    public long Id { get; private set; }
    public long PhoneId { get; private set; }
    public int? FaxAreaCode { get; private set; }
    public int? FaxNumber { get; private set; }
    public string? EmailAddress { get; private set; }
}
