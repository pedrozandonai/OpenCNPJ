namespace OpenCnpj.Application.Contacts.Domain;
public class Contact
{
    public long ID { get; private set; }
    public long PhoneID { get; private set; }
    public int? FaxAreaCode { get; private set; }
    public int? FaxNumber { get; private set; }
    public string? EmailAddress { get; private set; }

    private Contact(long id, long phoneID, int? faxAreaCode, int? faxNumber, string? emailAddress)
    {
        ID = id;
        PhoneID = phoneID;
        FaxAreaCode = faxAreaCode;
        FaxNumber = faxNumber;
        EmailAddress = emailAddress;
    }

    public static Contact Create(long id, long phoneID, int? faxAreaCode, int? faxNumber, string? emailAddress)
        => new(id, phoneID, faxAreaCode, faxNumber, emailAddress);
}
