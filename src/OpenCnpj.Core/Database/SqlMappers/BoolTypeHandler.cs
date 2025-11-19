using Dapper;

namespace OpenCnpj.Core.Database.SqlMappers;

public class BoolTypeHandler : SqlMapper.TypeHandler<bool>
{
    public override bool Parse(object value)
    {
        return Convert.ToInt32(value) == 1;
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, bool value)
    {
        parameter.Value = value ? 't' : 'f';
    }
}
