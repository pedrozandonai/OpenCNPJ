using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class EconomicActivitiesCollection : CollectionBase, IMongoApplicationCollection
{
    public string CollectionName => "economic_activities";
}
