using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class CountryRawRecord : RawRecordBase, IMongoApplicationCollection
{
    public string CollectionName => "countries";
}
