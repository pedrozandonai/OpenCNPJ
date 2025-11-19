using MongoDB.Driver;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Companies.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.MongoApplicationCollections.Extensions;
public static class CompaniesCollectionExtension
{
    public static async Task<List<CompaniesCollection>> GetCompaniesByFilters(this IMongoDatabaseFactory mongoDatabaseFactory, string? baseCnpj = null, string? companyName = null, int? legalNatureCode = null, int? responsibleQualification = null, int? shareCapital = null, short? companySize = null, CancellationToken cancellationToken = default)
    {
        var companiesCollection = mongoDatabaseFactory.Database.GetCollection<CompaniesCollection>("companies");

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

        var finalFilter = filters.Any()
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        return await companiesCollection
            .Find(finalFilter)
            .ToListAsync(cancellationToken);
    }

    //public static CompanyDto BuildCompanyDto(CompaniesCollection companyCollectionRecord, LegalNaturesCollection legalNatureCollectionRecord)
    //    => new(new CnpjDto(companyCollectionRecord.BasicCnpj, null,null),
    //        companyCollectionRecord.CorporateName,
    //        );
        
}
