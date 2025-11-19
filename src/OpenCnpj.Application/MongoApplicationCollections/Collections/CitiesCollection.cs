using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]

public class CitiesCollection : CollectionBase, IMongoApplicationCollection
{
    public string CollectionName => "cities";
}
