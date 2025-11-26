using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class SimplesCollection : IMongoApplicationCollection
{
    public string CollectionName => "simples";
    [MongoIndex]
    public string BasicCnpj { get; set; } = string.Empty;
    [MongoIndex]
    public bool? OptInSimple { get; set; }
    public DateTime? SimpleOptionDate { get; set; }
    public DateTime? SimpleExclusionDate { get; set; }
    [MongoIndex]
    public bool? OptInMei { get; set; }
    public DateTime? MeiOptionDate { get; set; }
    public DateTime? MeiExclusionDate { get; set; }
}