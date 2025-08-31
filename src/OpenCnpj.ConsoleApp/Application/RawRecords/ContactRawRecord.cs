using MongoDB.Bson.Serialization.Attributes;

namespace OpenCnpj.ConsoleApp.Application.RawRecords;

[BsonIgnoreExtraElements]
public class ContactRawRecord
{
    public string PhoneAreaCode1 { get; set; }
    public string PhoneNumber1 { get; set; }
    public string PhoneAreaCode2 { get; set; }
    public string PhoneNumber2 { get; set; }
    public string FaxAreaCode { get; set; }
    public string FaxNumber { get; set; }
    public string Email { get; set; }
}
