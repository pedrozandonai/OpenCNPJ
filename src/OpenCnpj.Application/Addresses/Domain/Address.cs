using CSharpFunctionalExtensions;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.Addresses.Domain;

[PgTable("addresses")]
public class Address
{
    [PgColumn("id", 1)]
    public long ID { get; private set; }

    [PgColumn("address_type_id", 2)]
    public long AddressTypeID { get; private set; }

    [PgColumn("city_id", 3)]
    public long CityID { get; private set; }

    [PgColumn("street", 4)]
    public string Street { get; private set; }

    [PgColumn("number", 5)]
    public int? Number { get; private set; }

    [PgColumn("complement", 6)]
    public string? Complement { get; private set; }

    [PgColumn("neighborhood", 7)]
    public string Neightborhood { get; private set; }

    [PgColumn("zip_code", 8)]
    public int? ZipCode { get; private set; }

    [PgColumn("federal_unit", 9)]
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
