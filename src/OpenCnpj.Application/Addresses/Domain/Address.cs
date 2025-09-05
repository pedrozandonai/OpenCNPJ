using CSharpFunctionalExtensions;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Application.Cities.Domain;

namespace OpenCnpj.Application.Addresses.Domain;
public class Address
{
    public long Id { get; private set; }
    public AddressType AddressType { get; private set; }
    public City City { get; private set; }
    public string Street { get; private set; }
    public int? Number { get; private set; }
    public string? Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public int? ZipCode { get; private set; }
    public string FederalUnit { get; private set; }

    private Address(long id, AddressType addressType, City city, string street, int? number, string? complement, string neighborhood, int? zipCode, string federalUnit)
    {
        Id = id;
        AddressType = addressType;
        City = city;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        FederalUnit = federalUnit;
    }

    private Address()
    {
    }

    public static Address Create(long id, AddressType addressType, City city, string street, int? number, string? complement, string neighborhood, int? zipCode, string federalUnit)
        => new(id, addressType, city, street, number, complement, neighborhood, zipCode, federalUnit);

    public Result SetId(long id)
    {
        if (Id != 0)
            return Result.Failure("The Id for the record 'Address' already has been set.");

        Id = id;

        return Result.Success();
    }
}
