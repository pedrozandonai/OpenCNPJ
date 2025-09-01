using CsvHelper.Configuration;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using System.Globalization;

namespace OpenCnpj.ConsoleApp.Mappers;
public class PartnerMapper : ClassMap<PartnerRawRecord>
{
    public PartnerMapper()
    {
        Map(p => p.BasicCnpj).Index(0);
        Map(p => p.PartnerType).Index(1);
        Map(p => p.PartnerName).Index(2);
        Map(p => p.PartnerDocument).Index(3);
        Map(p => p.PartnerQualification).Index(4);
        Map(p => p.EntryDate).Index(5).Convert(c =>
        {
            var val = c.Row.GetField(5);
            if (DateOnly.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        }); ;
        Map(p => p.CountryCode).Index(6);
        Map(p => p.RepresentativeDocument).Index(7);
        Map(p => p.RepresentativeName).Index(8);
        Map(p => p.RepresentativeQualification).Index(9);
        Map(p => p.AgeRange).Index(10);
    }
}
