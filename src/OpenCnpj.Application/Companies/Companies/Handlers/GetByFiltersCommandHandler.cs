using CSharpFunctionalExtensions;
using OpenCnpj.Application.Cnpjs.Models.Dtos;
using OpenCnpj.Application.Companies.Companies.Commands;
using OpenCnpj.Application.Companies.Companies.Models.Dtos;
using OpenCnpj.Application.Establishments.Models.Dtos;
using OpenCnpj.Application.LegalNatures.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Extensions;
using OpenCnpj.Application.Paginations.Models;
using OpenCnpj.Application.Partners.Models.Dtos;
using OpenCnpj.Application.Qualifications.Models.Dtos;
using OpenCnpj.Core;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Companies.Companies.Handlers;
public class GetByFiltersCommandHandler(IMongoDatabaseFactory mongoDatabaseFactory) : IRequestHandler<GetByFiltersCommand, Result<PaginationViewModel<CompanyDto>>>
{
    public async Task<Result<PaginationViewModel<CompanyDto>>> Handle(GetByFiltersCommand request, CancellationToken cancellationToken)
    {
        var requestValidationResult = request.Validate();
        if (requestValidationResult.IsFailure)
            return Result.Failure<PaginationViewModel<CompanyDto>>(requestValidationResult.Error);

        var page = request.Page ?? 1;
        var limit = request.Limit ?? 25;

        List<CompanyDto> companyDtos = [];

        var filteredCompanies = await mongoDatabaseFactory.GetCompaniesByFilters(request.BaseCnpj, request.CompanyName, request.LegalNatureCode, request.ResponsibleQualification, request.ShareCapital, request.CompanySize, request.Page, request.Limit, cancellationToken);
        if (filteredCompanies.Count <= 0)
            return Result.Success(new PaginationViewModel<CompanyDto>([], new PaginationDto(page, limit, 0)));

        foreach(var filteredCompany in filteredCompanies)
        {
            var cnpj = new CnpjDto(filteredCompany.BasicCnpj, null, null);

            var companyLegalNature = await mongoDatabaseFactory.GetFilteredCollectionBase<LegalNaturesCollection>(code: filteredCompany.LegalNatureCode, page:request.Page, limit:request.Limit, cancellationToken: cancellationToken);

            var companyLegalNatureDto = companyLegalNature.FirstOrDefault().CreateDtoByCollectionBase<LegalNaturesCollection, LegalNatureDto>();

            var companyResponsibleQualification = await mongoDatabaseFactory.GetFilteredCollectionBase<QualificationsCollection>(code: filteredCompany.ResponsibleQualification, page: request.Page, limit: request.Limit, cancellationToken: cancellationToken);

            var companyResponsibleQualificationDto = companyResponsibleQualification.FirstOrDefault().CreateDtoByCollectionBase<QualificationsCollection, QualificationDto>();

            var companySimples = await mongoDatabaseFactory.GetSimplesByFilters(baseCnpj: filteredCompany.BasicCnpj, page: request.Page, limit: request.Limit, cancellationToken: cancellationToken);

            var simplesDto = companySimples.FirstOrDefault().BuildSimplesDto();

            var partnersCollection = await mongoDatabaseFactory.GetPartnersByFilters(cnpj: cnpj, page: request.Page, limit: request.Limit, cancellationToken: cancellationToken);

            List<PartnerDto> partners = [];
            foreach (var partnerRecord in partnersCollection)
            {
                var partnerDto = await mongoDatabaseFactory.BuildPartnerDto(partnerRecord, cancellationToken);
                if (partnerDto == null)
                    continue;

                partners.Add(partnerDto);
            }

            var establishmentsCollection = await mongoDatabaseFactory.GetEstablishmentsByFilters(cnpj: cnpj, page: request.Page, limit: request.Limit, cancellationToken: cancellationToken);

            List<EstablishmentDto> establishments = [];
            foreach (var establishmentRecord in establishmentsCollection)
            {
                var establishmentDto = await mongoDatabaseFactory.BuildEstablishmentDto(establishmentRecord, cancellationToken);
                if (establishmentDto == null)
                    continue;

                establishments.Add(establishmentDto);
            }

            companyDtos.Add(filteredCompany.BuildCompanyDto(companyLegalNatureDto, companyResponsibleQualificationDto, simplesDto, partners, establishments));
        }

        var pagination = new PaginationDto(page, limit, companyDtos.Count);

        return Result.Success(new PaginationViewModel<CompanyDto>(companyDtos, pagination));
    }
}
