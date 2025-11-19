using MongoDB.Driver;
using OpenCnpj.Core.Attributes;
using System.Reflection;

namespace OpenCnpj.Core.Helpers;

public static class MongoIndexHelper
{
    public static async Task EnsureIndexesForType<T>(
        IMongoCollection<T> collection,
        CancellationToken cancellationToken = default)
    {
        var indexModels = new List<CreateIndexModel<T>>();

        var props = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(MongoIndexAttribute), false).Length != 0)
            .ToList();

        foreach (var prop in props)
        {
            var attr = prop.GetCustomAttribute<MongoIndexAttribute>()!;
            var keyDef = Builders<T>.IndexKeys.Ascending(prop.Name);

            var model = new CreateIndexModel<T>(
                keyDef,
                new CreateIndexOptions
                {
                    Name = $"{prop.Name}_asc",
                    Unique = attr.Unique
                });

            indexModels.Add(model);
        }

        if (indexModels.Count > 0)
            await collection.Indexes.CreateManyAsync(indexModels, cancellationToken);
    }
}
