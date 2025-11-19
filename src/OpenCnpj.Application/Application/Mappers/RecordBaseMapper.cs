using CsvHelper.Configuration;
using OpenCnpj.Application.MongoApplicationCollections.Collections;

namespace OpenCnpj.Application.Application.Mappers;
public class RawRecordBaseMap<T> : ClassMap<T> where T : CollectionBase
{
    public RawRecordBaseMap()
    {
        Map(r => r.Code).Index(0);
        Map(r => r.Description).Index(1);
    }
}