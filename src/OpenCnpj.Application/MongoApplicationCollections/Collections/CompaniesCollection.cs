using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class CompaniesCollection : IMongoApplicationCollection
{
    public string CollectionName => "companies";

    [MongoIndex(unique: true)]
    public string BasicCnpj { get; set; } = string.Empty;
    [MongoIndex]
    public string CorporateName { get; set; } = string.Empty;
    [MongoIndex]
    public int LegalNatureCode { get; set; }
    [MongoIndex]
    public int ResponsibleQualification { get; set; }
    public decimal ShareCapital { get; set; }
    [MongoIndex]
    public short? CompanySize { get; set; }
    public string ResponsibleFederativeEntity { get; set; } = string.Empty;
}
