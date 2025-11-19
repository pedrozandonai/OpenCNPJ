using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class AddressesCollection : IMongoApplicationCollection
{
    public string CollectionName => "addresses";
    public string StreetType { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string AdditionalAddressInfo { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string MunicipalityCode { get; set; } = string.Empty;
}
