using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.Application.RawRecords;

namespace OpenCnpj.Application.Companies.Models;
[BsonIgnoreExtraElements]
public class CompanyWithEstablishments : CompanyRawRecord
{
    public List<EstablishmentRawRecord> Establishments { get; set; } = [];
}

