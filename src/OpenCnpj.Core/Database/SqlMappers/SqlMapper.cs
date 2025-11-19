using Dapper;

namespace OpenCnpj.Core.Database.SqlMappers;

public static class SqlMappers
{
    public static void AddSqlMappers()
    {
        SqlMapper.AddTypeHandler(new BoolTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(bool));
        SqlMapper.RemoveTypeMap(typeof(bool?));
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(DateOnly));
        SqlMapper.RemoveTypeMap(typeof(DateOnly?));
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(TimeOnly));
        SqlMapper.RemoveTypeMap(typeof(TimeOnly?));
        SqlMapper.AddTypeHandler(new TimeSpanHandler());
        SqlMapper.RemoveTypeMap(typeof(TimeSpan));
        SqlMapper.RemoveTypeMap(typeof(TimeSpan?));
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
    }
}
