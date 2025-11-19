using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class CnaeRawRecord : RawRecordBase, IMongoApplicationCollection
{
    public string CollectionName => "economic_activities";
}
