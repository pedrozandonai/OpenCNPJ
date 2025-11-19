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
    public string CorporateName { get; set; } = string.Empty;
    public int LegalNatureCode { get; set; }
    public int ResponsibleQualification { get; set; }
    public decimal ShareCapital { get; set; }
    public short? CompanySize { get; set; }
    public string ResponsibleFederativeEntity { get; set; } = string.Empty;
}
