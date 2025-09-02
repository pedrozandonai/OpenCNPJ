using MongoDB.Bson;
using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;

namespace OpenCnpj.ConsoleApp.Core.Database.Migrations;
public static class MongoIndexes
{
    public static async Task EnsureIndexes(IMongoDatabaseFactory mongoDatabaseFactory)
    {
        var db = mongoDatabaseFactory.Database;

        var establishments = db.GetCollection<BsonDocument>("EstablishmentsRaw");

        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("BasicCnpj");

        var indexModel = new CreateIndexModel<BsonDocument>(
            indexKeys,
            new CreateIndexOptions { Name = "BasicCnpj_asc" }
        );

        await establishments.Indexes.CreateOneAsync(indexModel);
    }
}
