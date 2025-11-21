using MongoDB.Driver;
using OpenCnpj.Application.Ages.Models.Dtos;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Countries.Models.Dtos;
using OpenCnpj.Application.LegalRepresentatives.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Application.Partners.Models.Dtos;
using OpenCnpj.Application.PartnerTypes.Models.Dtos;
using OpenCnpj.Application.Qualifications.Models.Dtos;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class PartnersCollectionExtension
{
    public static async Task<List<PartnersCollection>> GetPartnersByFilters(this IMongoDatabaseFactory mongoDatabaseFactory, CnpjDto? cnpj = null, short? partnerType = null, string? partnerName = null, string? partnerDocument = null, string? partnerQualification = null, DateTime? entryDate = null, string? countryCode = null, string? representativeDocument = null, string? representativeName = null, string? representativeQualification = null, string? ageRange = null, int? page = 1, int? limit = 25, CancellationToken cancellationToken = default)
    {
        var collectionName = ((IMongoApplicationCollection)Activator.CreateInstance(typeof(PartnersCollection))).CollectionName;

        var partnersFilteredCollection = mongoDatabaseFactory.Database.GetCollection<PartnersCollection>(collectionName);

        var filterBuilder = Builders<PartnersCollection>.Filter;
        var filters = new List<FilterDefinition<PartnersCollection>>();

        if (cnpj != null && !string.IsNullOrWhiteSpace(cnpj.BaseCnpj))
            filters.Add(filterBuilder.Eq(c => c.BasicCnpj, cnpj.BaseCnpj));

        if (partnerType.HasValue)
            filters.Add(filterBuilder.Eq(c => c.PartnerType, partnerType.Value));

        if (!string.IsNullOrEmpty(partnerName))
            filters.Add(filterBuilder.Eq(c => c.PartnerName, partnerName));

        if (!string.IsNullOrEmpty(partnerDocument))
            filters.Add(filterBuilder.Eq(c => c.PartnerDocument, partnerDocument));

        if (!string.IsNullOrEmpty(partnerQualification))
            filters.Add(filterBuilder.Eq(c => c.PartnerQualification, partnerQualification));

        if (entryDate.HasValue)
            filters.Add(filterBuilder.Eq(c => c.EntryDate, entryDate.Value));

        if (!string.IsNullOrEmpty(countryCode))
            filters.Add(filterBuilder.Eq(c => c.CountryCode, countryCode));

        if (!string.IsNullOrEmpty(representativeDocument))
            filters.Add(filterBuilder.Eq(c => c.RepresentativeDocument, representativeDocument));

        if (!string.IsNullOrEmpty(representativeName))
            filters.Add(filterBuilder.Eq(c => c.RepresentativeName, representativeName));

        if (!string.IsNullOrEmpty(representativeQualification))
            filters.Add(filterBuilder.Eq(c => c.RepresentativeQualification, representativeQualification));

        if (!string.IsNullOrEmpty(ageRange))
            filters.Add(filterBuilder.Eq(c => c.AgeRange, ageRange));

        var finalFilter = filters.Count != 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await partnersFilteredCollection
            .Find(finalFilter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public static async Task<PartnerDto?> BuildPartnerDto(this IMongoDatabaseFactory mongoDatabaseFactory, PartnersCollection? partnersCollectionRecord, CancellationToken cancellationToken)
    {
        if (partnersCollectionRecord == null)
            return null;

        var partnerQualification = await mongoDatabaseFactory.GetFilteredCollectionBase<QualificationsCollection>(code: int.Parse(partnersCollectionRecord.PartnerQualification), cancellationToken: cancellationToken);

        var partnerQualificationDto = partnerQualification.FirstOrDefault().CreateDtoByCollectionBase<QualificationsCollection, QualificationDto>();

        CountryDto? countryDto = null;
        if (!string.IsNullOrEmpty(partnersCollectionRecord.CountryCode))
        {
            var partnerCountry = await mongoDatabaseFactory.GetFilteredCollectionBase<CountriesCollection>(code: int.Parse(partnersCollectionRecord.CountryCode), cancellationToken: cancellationToken);

            countryDto = partnerCountry.FirstOrDefault().CreateDtoByCollectionBase<CountriesCollection, CountryDto>();
        }

        RepresentativeDto? representativeDto = null;
        if (!string.IsNullOrEmpty(partnersCollectionRecord.RepresentativeDocument) &&
            !string.IsNullOrEmpty(partnersCollectionRecord.RepresentativeName) &&
            !string.IsNullOrEmpty(partnersCollectionRecord.RepresentativeQualification))
        {
            var representativeQualification = await mongoDatabaseFactory.GetFilteredCollectionBase<QualificationsCollection>(code: int.Parse(partnersCollectionRecord.RepresentativeQualification), cancellationToken: cancellationToken);

            var representativeQualificationDto = representativeQualification.FirstOrDefault().CreateDtoByCollectionBase<QualificationsCollection, QualificationDto>();

            representativeDto = new RepresentativeDto(partnersCollectionRecord.RepresentativeDocument, partnersCollectionRecord.RepresentativeName, representativeQualificationDto);
        }

        return new(PartnerTypeDto.CreateByID(partnersCollectionRecord.PartnerType),
            partnersCollectionRecord.PartnerName,
            partnersCollectionRecord.PartnerDocument,
            partnerQualificationDto,
            partnersCollectionRecord.EntryDate,
            countryDto,
            representativeDto,
            AgeDto.CreateByID(short.Parse(partnersCollectionRecord.AgeRange)));
    }
}

