using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.Application.RawRecords;

[BsonIgnoreExtraElements]
public class ContactRawRecord
{
    public string PhoneAreaCode1 { get; set; } = string.Empty;
    public string PhoneNumber1 { get; set; } = string.Empty;
    public string PhoneAreaCode2 { get; set; } = string.Empty;
    public string PhoneNumber2 { get; set; } = string.Empty;
    public string FaxAreaCode { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
