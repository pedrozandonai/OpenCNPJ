using CsvHelper.Configuration;
using OpenCnpj.Application.RawRecords;
using System.Globalization;

namespace OpenCnpj.ConsoleApp.Mappers;
public class SimpleDataMapper : ClassMap<SimpleDataRawRecord>
{
    public SimpleDataMapper()
    {
        Map(s => s.BasicCnpj).Index(0);
        Map(s => s.OptInSimple).Index(1).Convert(c =>
        {
            var val = c.Row.GetField(1);

            if (val.Equals("S", StringComparison.InvariantCultureIgnoreCase)) return true;
            if (val.Equals("N", StringComparison.InvariantCultureIgnoreCase)) return false;

            return null;
        });
        Map(s => s.SimpleOptionDate).Index(2).Convert(c =>
        {
            var val = c.Row.GetField(2);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
        Map(s => s.SimpleExclusionDate).Index(3).Convert(c =>
        {
            var val = c.Row.GetField(3);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
        Map(s => s.OptInMei).Index(4).Convert(c =>
        {
            var val = c.Row.GetField(4);

            if (val.Equals("S", StringComparison.InvariantCultureIgnoreCase)) return true;
            if (val.Equals("N", StringComparison.InvariantCultureIgnoreCase)) return false;

            return null;
        });
        Map(s => s.MeiOptionDate).Index(5).Convert(c =>
        {
            var val = c.Row.GetField(5);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
        Map(s => s.MeiExclusionDate).Index(6).Convert(c =>
        {
            var val = c.Row.GetField(6);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
    }
}
