using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.ConsoleApp.Application.Enums;

namespace OpenCnpj.ConsoleApp.Application.RawRecords;

[BsonIgnoreExtraElements]
public class CompanyRawRecord
{
    public string BasicCnpj { get; set; } = string.Empty;
    public string CorporateName { get; set; } = string.Empty;
    public int LegalNatureCode { get; set; }
    public int ResponsibleQualification { get; set; }
    public decimal ShareCapital { get; set; }
    public ECompanySize? CompanySize { get; set; }
    public string ResponsibleFederativeEntity { get; set; } = string.Empty; // Não sei o que vai aqui, mas ta lá
}
