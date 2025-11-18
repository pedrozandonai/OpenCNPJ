using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.Establishments.Models;
using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.Application.Companies.Models;
[BsonIgnoreExtraElements]
public class CompanyDetailedInformation : CompanyRawRecord
{
    public List<EstablishmentDetailedInformation> Establishments { get; set; } = [];
    public List<PartnerRawRecord> Partners { get; set; } = [];
    public List<SimpleDataRawRecord> SimpleData { get; set; } = [];
    public LegalNatureRawRecord LegalNature { get; set; } = null!;
    public PartnerQualificationRawRecord PartnerQualification { get; set; } = null!;
}

