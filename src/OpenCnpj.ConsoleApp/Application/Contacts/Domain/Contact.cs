namespace OpenCnpj.ConsoleApp.Application.Contacts.Domain;
public class Contact
{
    public long ID { get; private set; }
    public long PhoneID { get; private set; }
    public int? FaxAreaCode { get; private set; }
    public int? FaxNumber { get; private set; }
    public string? EmailAddress { get; private set; }
}
