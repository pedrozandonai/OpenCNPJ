using NpgsqlTypes;

namespace OpenCnpj.Core.Mappers;

public static class PgTypeMap
{
    public static bool TryGetDbType(Type t, out NpgsqlDbType dbType)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;

        if (t == typeof(int)) { dbType = NpgsqlDbType.Integer; return true; }
        if (t == typeof(long)) { dbType = NpgsqlDbType.Bigint; return true; }
        if (t == typeof(short)) { dbType = NpgsqlDbType.Smallint; return true; }
        if (t == typeof(bool)) { dbType = NpgsqlDbType.Boolean; return true; }
        if (t == typeof(string)) { dbType = NpgsqlDbType.Text; return true; }
        if (t == typeof(decimal)) { dbType = NpgsqlDbType.Numeric; return true; }
        if (t == typeof(double)) { dbType = NpgsqlDbType.Double; return true; }
        if (t == typeof(float)) { dbType = NpgsqlDbType.Real; return true; }
        if (t == typeof(DateTime)) { dbType = NpgsqlDbType.Timestamp; return true; }
        if (t.IsEnum) { dbType = NpgsqlDbType.Integer; return true; }

        dbType = default;

        return false;
    }
}

