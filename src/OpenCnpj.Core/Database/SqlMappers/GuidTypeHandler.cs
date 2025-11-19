using Dapper;
using System.Data;

namespace OpenCnpj.Core.Database.SqlMappers;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value)
    {
        var stringValue = value.ToString();
        return string.IsNullOrEmpty(stringValue) ? Guid.Empty : Guid.Parse(stringValue);
    }

    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }
}
