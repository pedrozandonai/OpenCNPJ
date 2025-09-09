using NpgsqlTypes;
using System.Reflection;

namespace OpenCnpj.Core.Models;

public sealed class PgColumnDescriptor
{
    public required string ColumnName { get; init; }
    public required PropertyInfo Property { get; init; }
    public required NpgsqlDbType DbType { get; init; }
    public int Order { get; init; }
}

