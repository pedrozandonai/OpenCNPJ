using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.Enums;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class PartnerRawRecord
{
    public string BasicCnpj { get; set; }
    public EParterType PartnerType { get; set; }
    public string PartnerName { get; set; }
    public string PartnerDocument { get; set; } // CPF or CNPJ
    public string PartnerQualification { get; set; }
    public DateOnly? EntryDate { get; set; }
    public string CountryCode { get; set; }
    public string RepresentativeDocument { get; set; }
    public string RepresentativeName { get; set; }
    public string RepresentativeQualification { get; set; }
    public string AgeRange { get; set; } // 0=Not applicable, 1=0-12, 2=13-20, etc.
}

