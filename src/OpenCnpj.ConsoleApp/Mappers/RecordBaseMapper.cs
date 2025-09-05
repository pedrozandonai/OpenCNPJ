using CsvHelper.Configuration;
using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.ConsoleApp.Mappers;
public class RecordBaseMapper : ClassMap<RawRecordBase>
{
    public RecordBaseMapper()
    {
        Map(r => r.Code).Index(0);
        Map(r => r.Description).Index(1);
    }
}
