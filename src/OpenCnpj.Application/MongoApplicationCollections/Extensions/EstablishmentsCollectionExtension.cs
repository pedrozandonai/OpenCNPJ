using MongoDB.Driver;
using OpenCnpj.Application.Addresses.Models.Dtos;
using OpenCnpj.Application.Cities.Models.Dtos;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Companies.CompanySituations.Models.Dtos;
using OpenCnpj.Application.Companies.CompanyTypes.Models.Dtos;
using OpenCnpj.Application.Contacts.Models.Dtos;
using OpenCnpj.Application.EconomicActivities.Models.Dtos;
using OpenCnpj.Application.Establishments.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Application.Reasons.Models.Dtos;
using OpenCnpj.Application.SpecialSituations.Models.Dtos;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class EstablishmentsCollectionExtension
{
    public static async Task<List<EstablishmentsCollection>> GetEstablishmentsByFilters(
        this IMongoDatabaseFactory mongoDatabaseFactory,
        CnpjDto? cnpj = null,
        short? companyType = null,
        string? tradeName = null,
        string? registrationStatus = null,
        DateTime? registrationStatusDate = null,
        string? registrationStatusReason = null,
        string? foreignCityName = null,
        string? countryCode = null,
        DateTime? startActivityDate = null,
        string? mainCnae = null,
        string? secondaryCnaes = null,
        AddressesCollection? address = null,
        ContactsCollection? contact = null,
        string? specialStatus = null,
        DateTime? specialStatusDate = null,
        int? page = 1,
        int? limit = 25,
        CancellationToken cancellationToken = default)
    {
        var collectionName =
            ((IMongoApplicationCollection)Activator.CreateInstance(typeof(EstablishmentsCollection))).CollectionName;

        var establishments = mongoDatabaseFactory.Database
            .GetCollection<EstablishmentsCollection>(collectionName);

        var filterBuilder = Builders<EstablishmentsCollection>.Filter;
        var filters = new List<FilterDefinition<EstablishmentsCollection>>();

        // --- CNPJ ---
        if (cnpj != null)
        {
            if (!string.IsNullOrWhiteSpace(cnpj.BaseCnpj))
                filters.Add(filterBuilder.Eq(c => c.BasicCnpj, cnpj.BaseCnpj));

            if (!string.IsNullOrWhiteSpace(cnpj.OrderCnpj))
                filters.Add(filterBuilder.Eq(c => c.OrderCnpj, cnpj.OrderCnpj));

            if (!string.IsNullOrWhiteSpace(cnpj.VerifierDigits))
                filters.Add(filterBuilder.Eq(c => c.CheckDigitCnpj, cnpj.VerifierDigits));
        }

        if (companyType.HasValue)
            filters.Add(filterBuilder.Eq(e => e.EstablishmentType, companyType.Value));

        // --- TradeName ---
        if (!string.IsNullOrWhiteSpace(tradeName))
            filters.Add(filterBuilder.Regex(c => c.TradeName,
                new MongoDB.Bson.BsonRegularExpression(tradeName, "i")));

        // --- RegistrationStatus ---
        if (!string.IsNullOrWhiteSpace(registrationStatus))
            filters.Add(filterBuilder.Eq(c => c.RegistrationStatus, registrationStatus));

        if (registrationStatusDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.RegistrationStatusDate, registrationStatusDate));

        if (!string.IsNullOrWhiteSpace(registrationStatusReason))
            filters.Add(filterBuilder.Eq(c => c.RegistrationStatusReason, registrationStatusReason));

        // --- ForeignCity + Country ---
        if (!string.IsNullOrWhiteSpace(foreignCityName))
            filters.Add(filterBuilder.Regex(c => c.ForeignCityName,
                new MongoDB.Bson.BsonRegularExpression(foreignCityName, "i")));

        if (!string.IsNullOrWhiteSpace(countryCode))
            filters.Add(filterBuilder.Eq(c => c.CountryCode, countryCode));

        // --- StartActivity ---
        if (startActivityDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.StartActivityDate, startActivityDate));

        // --- CNAE ---
        if (!string.IsNullOrWhiteSpace(mainCnae))
            filters.Add(filterBuilder.Eq(c => c.MainCnae, mainCnae));

        if (!string.IsNullOrWhiteSpace(secondaryCnaes))
            filters.Add(filterBuilder.Regex(c => c.SecondaryCnaes,
                new MongoDB.Bson.BsonRegularExpression(secondaryCnaes, "i")));

        // --- Address ---
        if (address != null)
            filters.Add(filterBuilder.Eq(c => c.Address, address));

        // --- Contact ---
        if (contact != null)
            filters.Add(filterBuilder.Eq(c => c.Contact, contact));

        // --- Special ---
        if (!string.IsNullOrWhiteSpace(specialStatus))
            filters.Add(filterBuilder.Eq(c => c.SpecialStatus, specialStatus));

        if (specialStatusDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.SpecialStatusDate, specialStatusDate));

        var finalFilter = filters.Count > 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await establishments
            .Find(finalFilter)
            .Skip((page!.Value - 1) * limit!.Value)
            .Limit(limit.Value)
            .ToListAsync(cancellationToken);
    }

    public static async Task<EstablishmentDto> BuildEstablishmentDto(this IMongoDatabaseFactory mongoDatabaseFactory, EstablishmentsCollection establishmentsCollectionRecord, CancellationToken cancellationToken)
    {
        ReasonDto? reasonDto = null;
        var situationReasonCode = int.Parse(establishmentsCollectionRecord.RegistrationStatusReason);
        if (situationReasonCode != 0)
        {
            var reasonsCollection = await mongoDatabaseFactory.GetFilteredCollectionBase<ReasonsCollection>(code: situationReasonCode, cancellationToken: cancellationToken);

            reasonDto = reasonsCollection.FirstOrDefault().CreateDtoByCollectionBase<ReasonsCollection, ReasonDto>();
        }

        var citiesCollection = await mongoDatabaseFactory.GetFilteredCollectionBase<CitiesCollection>(code: int.Parse(establishmentsCollectionRecord.Address.MunicipalityCode), cancellationToken: cancellationToken);

        var cityDto = citiesCollection.FirstOrDefault().CreateDtoByCollectionBase<CitiesCollection, CityDto>();

        var mainEconomicActivityCollection = await mongoDatabaseFactory.GetFilteredCollectionBase<EconomicActivitiesCollection>(code: int.Parse(establishmentsCollectionRecord.MainCnae), cancellationToken: cancellationToken);

        var mainEconomicActivityDto = mainEconomicActivityCollection.FirstOrDefault().CreateDtoByCollectionBase<EconomicActivitiesCollection, EconomicActivityDto>();

        List<EconomicActivityDto> secondaryEconomicActivities = [];
        var secondaryEconomicActivitiesCodes = establishmentsCollectionRecord.SecondaryCnaes.Split(",");
        if (secondaryEconomicActivitiesCodes.Length != 0)
        {
            foreach(var secondaryEconomicActivityCode in secondaryEconomicActivitiesCodes)
            {
                if (string.IsNullOrEmpty(secondaryEconomicActivityCode))
                    continue;

                var secondaryEconomicActivitiesCollection = await mongoDatabaseFactory.GetFilteredCollectionBase<EconomicActivitiesCollection>(code: int.Parse(secondaryEconomicActivityCode), cancellationToken: cancellationToken);

                var secondaryEconomicActivityDto = secondaryEconomicActivitiesCollection.FirstOrDefault().CreateDtoByCollectionBase<EconomicActivitiesCollection, EconomicActivityDto>();

                secondaryEconomicActivities.Add(secondaryEconomicActivityDto);
            }
        }

        List<PhoneDto> contactPhones = [];
        if (!string.IsNullOrEmpty(establishmentsCollectionRecord.Contact.PhoneAreaCode1) &&
            !string.IsNullOrEmpty(establishmentsCollectionRecord.Contact.PhoneNumber1))
            contactPhones.Add(new PhoneDto(establishmentsCollectionRecord.Contact.PhoneAreaCode1, establishmentsCollectionRecord.Contact.PhoneNumber1));

        if (!string.IsNullOrEmpty(establishmentsCollectionRecord.Contact.PhoneAreaCode2) &&
            !string.IsNullOrEmpty(establishmentsCollectionRecord.Contact.PhoneNumber2))
            contactPhones.Add(new PhoneDto(establishmentsCollectionRecord.Contact.PhoneAreaCode2, establishmentsCollectionRecord.Contact.PhoneNumber2));

        SpecialSituationDto? specialSituationDto = null;
        if (!string.IsNullOrEmpty(establishmentsCollectionRecord.SpecialStatus) &&
            establishmentsCollectionRecord.SpecialStatusDate.HasValue)
            specialSituationDto = new(establishmentsCollectionRecord.SpecialStatus, establishmentsCollectionRecord.SpecialStatusDate.Value);

        return new EstablishmentDto(
            new CnpjDto(establishmentsCollectionRecord.BasicCnpj, establishmentsCollectionRecord.OrderCnpj, establishmentsCollectionRecord.CheckDigitCnpj),
            CompanyTypeDto.CreateByID(establishmentsCollectionRecord.EstablishmentType),
            establishmentsCollectionRecord.TradeName,
            CompanySituationDto.CreateByID(short.Parse(establishmentsCollectionRecord.RegistrationStatus)),
            establishmentsCollectionRecord.RegistrationStatusDate ?? default,
            reasonDto,
            string.IsNullOrWhiteSpace(establishmentsCollectionRecord.ForeignCityName) ? null : establishmentsCollectionRecord.ForeignCityName,
            new AddressDto(cityDto, establishmentsCollectionRecord.Address.StreetType, establishmentsCollectionRecord.Address.StreetName, establishmentsCollectionRecord.Address.Number, establishmentsCollectionRecord.Address.District, establishmentsCollectionRecord.Address.ZipCode, establishmentsCollectionRecord.Address.State, establishmentsCollectionRecord.Address.AdditionalAddressInfo),
            establishmentsCollectionRecord.StartActivityDate ?? default,
            mainEconomicActivityDto,
            secondaryEconomicActivities,
            new ContactDto(contactPhones, establishmentsCollectionRecord.Contact.FaxAreaCode, establishmentsCollectionRecord.Contact.FaxNumber, establishmentsCollectionRecord.Contact.Email),
            specialSituationDto
        );
    }
}
