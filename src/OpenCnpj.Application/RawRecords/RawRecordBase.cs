using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Core.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public abstract class RawRecordBase
{
    [MongoIndex(unique: true)]
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
