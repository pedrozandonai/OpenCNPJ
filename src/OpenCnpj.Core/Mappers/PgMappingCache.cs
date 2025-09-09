using OpenCnpj.Core.Attributes;
using OpenCnpj.Core.Extensions;
using OpenCnpj.Core.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace OpenCnpj.Core.Mappers;

internal static class PgMappingCache
{
    private static readonly ConcurrentDictionary<Type, (string Table, PgColumnDescriptor[] Cols)> _cache = new();

    public static (string Table, PgColumnDescriptor[] Cols) GetMapping<T>()
    {
        var t = typeof(T);
        return _cache.GetOrAdd(t, static typ =>
        {
            var tableAttr = typ.GetCustomAttribute<PgTableAttribute>();
            var tableName = tableAttr?.Name ?? typ.Name.ToSnakeCase();

            var props = typ.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                           .Where(p => p.CanRead)
                           .Select(p =>
                           {
                               var colAttr = p.GetCustomAttribute<PgColumnAttribute>();
                               var name = colAttr?.Name ?? p.Name.ToSnakeCase();

                               if (!PgTypeMap.TryGetDbType(p.PropertyType, out var dbt))
                                   return null;

                               return new PgColumnDescriptor
                               {
                                   ColumnName = name,
                                   Property = p,
                                   DbType = dbt,
                                   Order = colAttr?.Order ?? 0
                               };
                           })
                           .Where(x => x != null)!
                           .OrderBy(x => x!.Order)
                           .ThenBy(x => x!.ColumnName)
                           .ToArray()!;

            return (tableName, props!);
        });
    }
}
