using CsvHelper.Configuration;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using System.Globalization;

namespace OpenCnpj.Application.MongoApplicationCollections.Mappers;
public class EstablishmentMapper : ClassMap<EstablishmentsCollection>
{
    public EstablishmentMapper()
    {
        Map(e => e.BasicCnpj).Index(0);
        Map(e => e.OrderCnpj).Index(1);
        Map(e => e.CheckDigitCnpj).Index(2);
        Map(e => e.HeadOfficeOrBranch).Index(3);
        Map(e => e.TradeName).Index(4);
        Map(e => e.RegistrationStatus).Index(5);
        Map(e => e.RegistrationStatusDate).Index(6).Convert(c =>
        {
            var val = c.Row.GetField(6);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
        Map(e => e.RegistrationStatusReason).Index(7);
        Map(e => e.ForeignCityName).Index(8);
        Map(e => e.CountryCode).Index(9);
        Map(e => e.StartActivityDate).Index(10).Convert(c =>
        {
            var val = c.Row.GetField(10);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
        Map(e => e.MainCnae).Index(11);
        Map(e => e.SecondaryCnaes).Index(12);
        Map(e => e.Address).Convert(c => new AddressesCollection
        {
            StreetType = c.Row.GetField(13),
            StreetName = c.Row.GetField(14),
            Number = c.Row.GetField(15),
            AdditionalAddressInfo = c.Row.GetField(16),
            District = c.Row.GetField(17),
            ZipCode = c.Row.GetField(18),
            State = c.Row.GetField(19),
            MunicipalityCode = c.Row.GetField(20)
        });
        Map(e => e.Contact).Convert(c => new ContactsCollection
        {
            PhoneAreaCode1 = c.Row.GetField(21),
            PhoneNumber1 = c.Row.GetField(22),
            PhoneAreaCode2 = c.Row.GetField(23),
            PhoneNumber2 = c.Row.GetField(24),
            FaxAreaCode = c.Row.GetField(25),
            FaxNumber = c.Row.GetField(26),
            Email = c.Row.GetField(27)
        });
        Map(e => e.SpecialStatus).Index(28);

        Map(e => e.SpecialStatusDate).Index(29).Convert(c =>
        {
            var val = c.Row.GetField(29);
            if (DateTime.TryParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        });
    }
}
