using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class SimpleDataRawRecord
{
    public string BasicCnpj { get; set; }
    public bool? OptInSimple { get; set; } // S, N, blank
    public DateTime? SimpleOptionDate { get; set; }
    public DateTime? SimpleExclusionDate { get; set; }
    public bool? OptInMei { get; set; } // S, N, blank
    public DateTime? MeiOptionDate { get; set; }
    public DateTime? MeiExclusionDate { get; set; }
}