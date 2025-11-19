using Dapper;
using System.Data;

namespace OpenCnpj.Core.Database.SqlMappers;

public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override TimeSpan Parse(object value)
    {
        string stringValue = (string)value;
        string[] parts = stringValue.Split(' ');

        int days = 0;
        TimeSpan time;

        if (parts.Length > 1)
        {
            days = int.Parse(parts[0]);
            time = TimeSpan.Parse(parts[1]);
        }
        else
        {
            time = TimeSpan.Parse(parts[0]);
        }

        return TimeSpan.FromDays(days) + time;
    }

    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = string.Format("{0:D1} {1:D2}:{2:D2}:{3:D2}.{4:D4}", value.Days, value.Hours, value.Minutes, value.Seconds, value.Milliseconds);
    }
}
