using CsvHelper.Configuration;
using OpenCnpj.Application.MongoApplicationCollections.Collections;

namespace OpenCnpj.Application.MongoApplicationCollections.Mappers;
public class CompanyMapper : ClassMap<CompaniesCollection>
{
    public CompanyMapper()
    {
        Map(c => c.BasicCnpj).Index(0);
        Map(c => c.CorporateName).Index(1);
        Map(c => c.LegalNatureCode).Index(2);
        Map(c => c.ResponsibleQualification).Index(3);
        Map(c => c.ShareCapital)
            .Index(4)
            .Convert(row =>
            {
                var raw = row.Row.GetField(4)?.Trim();

                if (string.IsNullOrWhiteSpace(raw))
                    return 0m;

                if (!long.TryParse(raw, out var value))
                    return 0m;

                return value / 100m;
            });
        Map(c => c.CompanySize).Index(5);
        Map(c => c.ResponsibleFederativeEntity).Index(6);
    }
}