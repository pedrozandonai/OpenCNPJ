using CsvHelper.Configuration;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using System.Globalization;

namespace OpenCnpj.Application.MongoApplicationCollections.Mappers;
public class CompanyMapper : ClassMap<CompaniesCollection>
{
    private readonly CultureInfo _brazillianCulture = new CultureInfo("pt-BR");

    public CompanyMapper()
    {
        Map(c => c.BasicCnpj).Index(0);
        Map(c => c.CorporateName).Index(1);
        Map(c => c.LegalNatureCode).Index(2);
        Map(c => c.ResponsibleQualification).Index(3);
        Map(c => c.ShareCapital)
            .Index(4)
            .Convert(args =>
            {
                var raw = args.Row.GetField(4);
                return Decimal.TryParse(raw, _brazillianCulture, out var decimalValue) ? decimalValue : 0;
            });
        Map(c => c.CompanySize).Index(5);
        Map(c => c.ResponsibleFederativeEntity).Index(6);
    }
}