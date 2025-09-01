using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.ConsoleApp.Application.RawRecords;

[BsonIgnoreExtraElements]
public abstract class RawRecordBase
{
    public string Code { get; set; }
    public string Description { get; set; }
}
