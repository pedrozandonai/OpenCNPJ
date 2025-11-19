using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class PartnerQualificationRawRecord : RawRecordBase, IMongoApplicationCollection
{
    public string CollectionName => "qualifications";
}
