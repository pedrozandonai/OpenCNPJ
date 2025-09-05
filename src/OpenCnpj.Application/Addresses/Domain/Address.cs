using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Addresses.Domain;
public class Address
{
    public long ID { get; private set; }
    public long AddressTypeID { get; private set; }
    public long CityID { get; private set; }
    public string Street { get; private set; }
    public int? Number { get; private set; }
    public string? Complement { get; private set; }
    public string Neightborhood { get; private set; }
    public int? ZipCode { get; private set; }
    public string FederalUnit { get; private set; }

    private Address(long iD, long addressTypeID, long cityID, string street, int? number, string? complement, string neightborhood, int? zipCode, string federalUnit)
    {
        ID = iD;
        AddressTypeID = addressTypeID;
        CityID = cityID;
        Street = street;
        Number = number;
        Complement = complement;
        Neightborhood = neightborhood;
        ZipCode = zipCode;
        FederalUnit = federalUnit;
    }

    public static Address Create(long id, long addressTypeID, long cityID, string street, int? number, string? complement, string neightborhood, int? zipCode, string federalUnit)
        => new(id, addressTypeID, cityID, street, number, complement, neightborhood, zipCode, federalUnit);

    public Result SetID(long id)
    {
        if (ID != 0)
            return Result.Failure("The ID for the record 'Address' already has been set.");

        ID = id;

        return Result.Success();
    }
}
