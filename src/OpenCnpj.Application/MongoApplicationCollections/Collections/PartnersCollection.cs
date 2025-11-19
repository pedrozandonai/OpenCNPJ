using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class PartnersCollection : IMongoApplicationCollection
{
    public string CollectionName => "partners";

    [MongoIndex]
    public string BasicCnpj { get; set; } = string.Empty;
    public short PartnerType { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerDocument { get; set; } = string.Empty;
    public string PartnerQualification { get; set; } = string.Empty;
    public DateTime? EntryDate { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string RepresentativeDocument { get; set; } = string.Empty;
    public string RepresentativeName { get; set; } = string.Empty;
    public string RepresentativeQualification { get; set; } = string.Empty;
    public string AgeRange { get; set; } = string.Empty;
}

