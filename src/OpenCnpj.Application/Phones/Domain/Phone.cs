namespace OpenCnpj.Application.Phones.Domain;
public class Phone
{
    public long ID { get; private set; }
    public int? AreaCode { get; private set; }
    public int? PhoneNumber { get; private set; }

    private Phone(long id, int? areaCode, int? phoneNumber)
    {
        ID = id;
        AreaCode = areaCode;
        PhoneNumber = phoneNumber;
    }

    public static Phone Create(long id, int? areaCode, int? phoneNumber)
        => new(id, areaCode, phoneNumber);
}
