using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class AddressRawRecord
{
    public string StreetType { get; set; }
    public string StreetName { get; set; }
    public string Number { get; set; }
    public string AdditionalAddressInfo { get; set; }
    public string District { get; set; }
    public string ZipCode { get; set; }
    public string State { get; set; }
    public string MunicipalityCode { get; set; }
}
