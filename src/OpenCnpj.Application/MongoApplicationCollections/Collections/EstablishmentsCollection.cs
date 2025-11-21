using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class EstablishmentsCollection : IMongoApplicationCollection
{
    public string CollectionName => "establishments";

    [MongoIndex]
    public string BasicCnpj { get; set; } = string.Empty;
    public string OrderCnpj { get; set; } = string.Empty;
    public string CheckDigitCnpj { get; set; } = string.Empty;
    public short HeadOfficeOrBranch { get; set; }
    [MongoIndex(unique: true)]
    public string TradeName { get; set; } = string.Empty;
    public string RegistrationStatus { get; set; } = string.Empty;
    public DateTime? RegistrationStatusDate { get; set; }
    public string RegistrationStatusReason { get; set; } = string.Empty;
    public string ForeignCityName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime? StartActivityDate { get; set; }
    public string MainCnae { get; set; } = string.Empty;
    public string SecondaryCnaes { get; set; } = string.Empty;
    public AddressesCollection Address { get; set; } = null!;
    public ContactsCollection Contact { get; set; } = null!;
    public string SpecialStatus { get; set; } = string.Empty;
    public DateTime? SpecialStatusDate { get; set; }
}

