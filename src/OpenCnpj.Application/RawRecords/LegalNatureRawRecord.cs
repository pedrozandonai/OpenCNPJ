using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class LegalNatureRawRecord : RawRecordBase, IMongoApplicationCollection
{
    public string CollectionName => "legal_natures";
}
