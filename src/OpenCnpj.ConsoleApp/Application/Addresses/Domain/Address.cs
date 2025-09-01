namespace OpenCnpj.ConsoleApp.Application.Addresses.Domain;
public class Address
{
    public long ID { get; private set; }
    public int AddressTypeID { get; private set; }
    public int CityID { get; private set; }
    public string Description { get; private set; }
    public int Number { get; private set; }
    public string? Complement { get; private set; }
    public string Neightborhood { get; private set; }
    public int CepNumber { get; private set; }
    public string FederalUnit { get; private set; }
}
