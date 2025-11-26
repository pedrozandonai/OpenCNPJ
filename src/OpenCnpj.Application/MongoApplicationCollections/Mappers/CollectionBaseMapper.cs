using CsvHelper.Configuration;
using OpenCnpj.Application.MongoApplicationCollections.Collections;

namespace OpenCnpj.Application.MongoApplicationCollections.Mappers;
public class RawRecordBaseMap<T> : ClassMap<T> where T : CollectionBase
{
    public RawRecordBaseMap()
    {
        Map(r => r.Code)
            .Index(0)
            .Convert(args =>
            {
                var raw = args.Row.GetField(0);
                return int.TryParse(raw, out var parsedValue) ? parsedValue : 0;
            });
        Map(r => r.Description).Index(1);
    }
}