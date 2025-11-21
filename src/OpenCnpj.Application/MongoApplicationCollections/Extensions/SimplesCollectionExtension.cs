using MongoDB.Driver;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Application.Simples.Models.Dtos;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class SimplesCollectionExtension
{
    public static async Task<List<SimplesCollection>> GetSimplesByFilters(this IMongoDatabaseFactory mongoDatabaseFactory, string? baseCnpj = null, bool? optInSimple = null, DateTime? simpleOptionDate = null, DateTime? simpleExclusionDate = null, bool? optInMei = null, DateTime? meiOptionDate = null, DateTime? meiExclusionDate = null, int? page = 1, int? limit = 25, CancellationToken cancellationToken = default)
    {
        var collectionName = ((IMongoApplicationCollection)Activator.CreateInstance(typeof(SimplesCollection))).CollectionName;

        var simplesFilteredCollection = mongoDatabaseFactory.Database.GetCollection<SimplesCollection>(collectionName);

        var filterBuilder = Builders<SimplesCollection>.Filter;
        var filters = new List<FilterDefinition<SimplesCollection>>();

        if (!string.IsNullOrWhiteSpace(baseCnpj))
            filters.Add(filterBuilder.Eq(c => c.BasicCnpj, baseCnpj));

        if (optInSimple.HasValue)
            filters.Add(filterBuilder.Eq(c => c.OptInSimple, optInSimple.Value));

        if (simpleOptionDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.SimpleOptionDate, simpleOptionDate.Value));

        if (simpleExclusionDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.SimpleExclusionDate, simpleExclusionDate.Value));

        if (optInMei.HasValue)
            filters.Add(filterBuilder.Eq(c => c.OptInMei, optInMei.Value));

        if (meiOptionDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.MeiOptionDate, meiOptionDate.Value));

        if (meiExclusionDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.MeiExclusionDate, meiExclusionDate.Value));

        var finalFilter = filters.Count != 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await simplesFilteredCollection
            .Find(finalFilter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public static SimplesDto BuildSimplesDto(this SimplesCollection? simpleCollectionRecord)
        => new(simpleCollectionRecord?.OptInSimple, simpleCollectionRecord?.SimpleOptionDate, simpleCollectionRecord?.SimpleExclusionDate, simpleCollectionRecord?.OptInMei, simpleCollectionRecord?.MeiOptionDate, simpleCollectionRecord?.MeiExclusionDate);
}
