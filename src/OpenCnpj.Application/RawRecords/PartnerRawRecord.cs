using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.Enums;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class PartnerRawRecord
{
    public string BasicCnpj { get; set; } = string.Empty;
    public EParterType PartnerType { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerDocument { get; set; } = string.Empty; // CPF or CNPJ
    public string PartnerQualification { get; set; } = string.Empty;
    public DateTime? EntryDate { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string RepresentativeDocument { get; set; } = string.Empty;
    public string RepresentativeName { get; set; } = string.Empty;
    public string RepresentativeQualification { get; set; } = string.Empty;
    public string AgeRange { get; set; } = string.Empty; // 0=Not applicable, 1=0-12, 2=13-20, etc.
}

