using MongoDB.Bson;
using MongoDB.Driver;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Core.Database.Migrations;
public static class MongoIndexes
{
    public static async Task EnsureIndexes(IMongoDatabaseFactory mongoDatabaseFactory)
    {
        var db = mongoDatabaseFactory.Database;

        var cities = db.GetCollection<BsonDocument>("CityRawRecord");
        var economicActivities = db.GetCollection<BsonDocument>("CnaeRawRecord");
        var companies = db.GetCollection<BsonDocument>("CompaniesRaw");
        var countries = db.GetCollection<BsonDocument>("CountryRawRecord");
        var establishments = db.GetCollection<BsonDocument>("EstablishmentsRaw");
        var legalNatures = db.GetCollection<BsonDocument>("LegalNatureRawRecord");
        var partnerQualifications = db.GetCollection<BsonDocument>("PartnerQualificationRawRecord");
        var partners = db.GetCollection<BsonDocument>("PartnersRaw");
        var reasons = db.GetCollection<BsonDocument>("ReasonRawRecord");
        var simples = db.GetCollection<BsonDocument>("SimplesRawRecord");

        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("BasicCnpj");

        var indexModel = new CreateIndexModel<BsonDocument>(
            indexKeys,
            new CreateIndexOptions { Name = "BasicCnpj_asc" }
        );

        await establishments.Indexes.CreateOneAsync(indexModel);
        await companies.Indexes.CreateOneAsync(indexModel);
        await partners.Indexes.CreateOneAsync(indexModel);
        await simples.Indexes.CreateOneAsync(indexModel);
    }
}
