using MongoDB.Driver;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Companies.Companies.Models.Dtos;
using OpenCnpj.Application.Companies.CompanySizes.Models.Dtos;
using OpenCnpj.Application.Establishments.Models.Dtos;
using OpenCnpj.Application.LegalNatures.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Application.Partners.Models.Dtos;
using OpenCnpj.Application.Qualifications.Models.Dtos;
using OpenCnpj.Application.Simples.Models.Dtos;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class CompaniesCollectionExtension
{
    public static async Task<List<CompaniesCollection>> GetCompaniesByFilters(this IMongoDatabaseFactory mongoDatabaseFactory, string? baseCnpj = null, string? companyName = null, int? legalNatureCode = null, int? responsibleQualification = null, int? shareCapital = null, short? companySize = null, int? page = 1, int? limit = 25, CancellationToken cancellationToken = default)
    {
        var collectionName = ((IMongoApplicationCollection)Activator.CreateInstance(typeof(CompaniesCollection))).CollectionName;

        var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompaniesCollection>(collectionName);

        var filterBuilder = Builders<CompaniesCollection>.Filter;
        var filters = new List<FilterDefinition<CompaniesCollection>>();

        if (!string.IsNullOrWhiteSpace(baseCnpj))
            filters.Add(filterBuilder.Eq(c => c.BasicCnpj, baseCnpj));

        if (!string.IsNullOrWhiteSpace(companyName))
            filters.Add(filterBuilder.Regex(c => c.CorporateName,
                new MongoDB.Bson.BsonRegularExpression(companyName, "i")));

        if (legalNatureCode.HasValue)
            filters.Add(filterBuilder.Eq(c => c.LegalNatureCode, legalNatureCode.Value));

        if (responsibleQualification.HasValue)
            filters.Add(filterBuilder.Eq(c => c.ResponsibleQualification, responsibleQualification.Value));

        if (shareCapital.HasValue)
            filters.Add(filterBuilder.Gte(c => c.ShareCapital, shareCapital.Value));

        if (companySize.HasValue)
            filters.Add(filterBuilder.Eq(c => c.CompanySize, companySize.Value));

        var finalFilter = filters.Count != 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await companiesCollection
            .Find(finalFilter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public static CompanyDto BuildCompanyDto(this CompaniesCollection companyCollectionRecord, LegalNatureDto legalNatureDto, QualificationDto partnerQualification, SimplesDto simples, IEnumerable<PartnerDto> partners, IEnumerable<EstablishmentDto> establishments)
        => new(new CnpjDto(companyCollectionRecord.BasicCnpj, null, null),
            companyCollectionRecord.CorporateName,
            legalNatureDto,
            partnerQualification,
            companyCollectionRecord.ShareCapital,
            CompanySizeDto.CreateByID(companyCollectionRecord.CompanySize),
            companyCollectionRecord.ResponsibleFederativeEntity,
            simples,
            partners,
            establishments);
}
