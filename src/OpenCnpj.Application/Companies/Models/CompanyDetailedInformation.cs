using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.MongoApplicationCollections.Collections;

namespace OpenCnpj.Application.Companies.Models;
[BsonIgnoreExtraElements]
public class CompanyDetailedInformation : CompaniesCollection
{
    public List<EstablishmentsCollection> Establishments { get; set; } = [];
    public List<PartnersCollection> Partners { get; set; } = [];
    public List<SimplesCollection> SimpleData { get; set; } = [];
    public LegalNaturesCollection LegalNature { get; set; } = null!;
    public QualificationsCollection PartnerQualification { get; set; } = null!;
}

