using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.Enums;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class EstablishmentRawRecord
{
    public string BasicCnpj { get; set; }
    public string OrderCnpj { get; set; }
    public string CheckDigitCnpj { get; set; }
    public ECompanyType HeadOfficeOrBranch { get; set; }
    public string TradeName { get; set; }
    public string RegistrationStatus { get; set; }
    public DateTime? RegistrationStatusDate { get; set; }
    public string RegistrationStatusReason { get; set; }
    public string ForeignCityName { get; set; }
    public string CountryCode { get; set; }
    public DateTime? StartActivityDate { get; set; }
    public string MainCnae { get; set; }
    public string SecondaryCnaes { get; set; }
    public AddressRawRecord Address { get; set; }
    public ContactRawRecord Contact { get; set; }
    public string SpecialStatus { get; set; }
    public DateTime? SpecialStatusDate { get; set; }
}

