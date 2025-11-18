using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.Enums;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class EstablishmentRawRecord
{
    public string BasicCnpj { get; set; } = string.Empty;
    public string OrderCnpj { get; set; } = string.Empty;
    public string CheckDigitCnpj { get; set; } = string.Empty;
    public ECompanyType HeadOfficeOrBranch { get; set; }
    public string TradeName { get; set; } = string.Empty;
    public string RegistrationStatus { get; set; } = string.Empty;
    public DateTime? RegistrationStatusDate { get; set; }
    public string RegistrationStatusReason { get; set; } = string.Empty;
    public string ForeignCityName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime? StartActivityDate { get; set; }
    public string MainCnae { get; set; } = string.Empty;
    public string SecondaryCnaes { get; set; } = string.Empty;
    public AddressRawRecord Address { get; set; } = null!;
    public ContactRawRecord Contact { get; set; } = null!;
    public string SpecialStatus { get; set; } = string.Empty;
    public DateTime? SpecialStatusDate { get; set; }
}

