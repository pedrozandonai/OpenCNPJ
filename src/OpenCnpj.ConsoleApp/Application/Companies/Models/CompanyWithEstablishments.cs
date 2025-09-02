using MongoDB.Bson.Serialization.Attributes;
using OpenCnpj.ConsoleApp.Application.RawRecords;

namespace OpenCnpj.ConsoleApp.Application.Companies.Models;
[BsonIgnoreExtraElements]
public class CompanyWithEstablishments : CompanyRawRecord
{
    public List<EstablishmentRawRecord> Establishments { get; set; } = [];
}

