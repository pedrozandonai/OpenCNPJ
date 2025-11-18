using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public abstract class RawRecordBase
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
