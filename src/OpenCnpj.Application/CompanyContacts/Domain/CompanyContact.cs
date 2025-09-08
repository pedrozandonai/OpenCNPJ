namespace OpenCnpj.Application.CompanyContacts.Domain;
public class CompanyContact
{
    public long CompanyID { get; set; }
    public long ContactID { get; set; }

    private CompanyContact(long companyID, long contactID)
    {
        CompanyID = companyID;
        ContactID = contactID;
    }

    public CompanyContact()
    {
    }

    public static CompanyContact Create(long companyID, long contactID)
        => new(companyID, contactID);
}
