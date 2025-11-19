using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Domain;

namespace OpenCnpj.Application.MongoApplicationCollections.Collections;

[BsonIgnoreExtraElements]
public class CountriesCollection : CollectionBase, IMongoApplicationCollection
{
    public string CollectionName => "countries";
}
