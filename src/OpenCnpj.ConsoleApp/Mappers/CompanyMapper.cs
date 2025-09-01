using CsvHelper.Configuration;
using OpenCnpj.ConsoleApp.Application.RawRecords;

namespace OpenCnpj.ConsoleApp.Mappers;
public class CompanyMapper : ClassMap<CompanyRawRecord>
{
    public CompanyMapper()
    {
        Map(c => c.BasicCnpj).Index(0);
        Map(c => c.CorporateName).Index(1);
        Map(c => c.LegalNatureCode).Index(2);
        Map(c => c.ResponsibleQualification).Index(3);
        Map(c => c.ShareCapital).Index(4);
        Map(c => c.CompanySize).Index(5);
        Map(c => c.ResponsibleFederativeEntity).Index(6);
    }
}