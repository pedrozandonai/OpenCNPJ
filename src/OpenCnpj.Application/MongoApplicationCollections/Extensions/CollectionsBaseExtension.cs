using MongoDB.Driver;
using OpenCnpj.Application.BaseRecords.Models;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class CollectionsBaseExtension
{
    public static async Task<List<T>> GetFilteredCollectionBase<T>(this IMongoDatabaseFactory mongoDatabaseFactory, int? code = null, string? description = null, int? page = 1, int? limit = 25, CancellationToken cancellationToken = default) where T : CollectionBase, IMongoApplicationCollection
    {
        var collectionName = ((IMongoApplicationCollection)Activator.CreateInstance(typeof(T))).CollectionName;

        var filteredCollectionBase = mongoDatabaseFactory.Database.GetCollection<T>(collectionName);

        var filterBuilder = Builders<T>.Filter;
        var filters = new List<FilterDefinition<T>>();

        if (code.HasValue)
            filters.Add(filterBuilder.Eq(c => c.Code, code.Value));

        if (!string.IsNullOrEmpty(description))
            filters.Add(filterBuilder.Eq(c => c.Description, description));

        var finalFilter = filters.Count != 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await filteredCollectionBase
            .Find(finalFilter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public static TDto CreateDtoByCollectionBase<TCollection, TDto>(this TCollection? collectionRecord)
        where TCollection : CollectionBase, IMongoApplicationCollection
        where TDto : BaseRecordDto, new()
    {
        if (collectionRecord == null)
            return new TDto
            {
                ID = default,
                Description = string.Empty
            };

        return new TDto
        {
            ID = collectionRecord.Code,
            Description = collectionRecord.Description
        };
    }
}
