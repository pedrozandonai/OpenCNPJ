using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public abstract class CollectionBase
{
    [MongoIndex(unique: true)]
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
