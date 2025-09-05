using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class SimpleDataRawRecord
{
    public string BasicCnpj { get; set; }
    public bool? OptInSimple { get; set; } // S, N, blank
    public DateOnly? SimpleOptionDate { get; set; }
    public DateOnly? SimpleExclusionDate { get; set; }
    public bool? OptInMei { get; set; } // S, N, blank
    public DateOnly? MeiOptionDate { get; set; }
    public DateOnly? MeiExclusionDate { get; set; }
}