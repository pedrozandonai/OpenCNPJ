using CSharpFunctionalExtensions;
using OpenCnpj.Application.Companies.Commands;
using OpenCnpj.Application.Companies.Models.Dtos;
using OpenCnpj.Application.MongoApplicationCollections.Extensions;
using OpenCnpj.Core;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Companies.Handlers;
public class GetByFiltersCommandHandler(IMongoDatabaseFactory mongoDatabaseFactory) : IRequestHandler<GetByFiltersCommand, Result<IEnumerable<CompanyDto>>>
{
    public async Task<Result<IEnumerable<CompanyDto>>> Handle(GetByFiltersCommand request, CancellationToken cancellationToken)
    {
        var filteredCompanies = await mongoDatabaseFactory.GetCompaniesByFilters(request.BaseCnpj, request.CompanyName, request.LegalNatureCode, request.ResponsibleQualification, request.ShareCapital, request.CompanySize, cancellationToken);
        if (filteredCompanies.Count <= 0)
            return Result.Success<IEnumerable<CompanyDto>>([]);

        List<CompanyDto> companyDtos = [];

        //var legalNature = await legalNaturesCollection
        //    .Find(c => c.Code == company.LegalNatureCode.ToString())
        //    .FirstOrDefaultAsync(cancellationToken);

        //var legalNatureDto = new LegalNatureDto(int.Parse(legalNature.Code), legalNature.Description);

        //var companyResponsibleQualification = await partnersQualificationsCollection
        //    .Find(c => c.Code == company.ResponsibleQualification.ToString())
        //    .FirstOrDefaultAsync(cancellationToken);

        //var mainPartnerQualification = new PartnerQualificationDto(int.Parse(companyResponsibleQualification.Code), companyResponsibleQualification.Description);

        //var simples = await simplesCollection
        //    .Find(c => c.BasicCnpj == company.BasicCnpj)
        //    .FirstOrDefaultAsync(cancellationToken);

        //var establishments = await establishmentsCollection
        //    .Find(c => c.BasicCnpj == company.BasicCnpj)
        //    .ToListAsync(cancellationToken);

        //List<EstablishmentDto> establishmentDtos = [];
        //foreach(var establishment in establishments)
        //{
        //    var registrationStatusReason = await reasonsCollection
        //        .Find(c => c.Code == establishment.RegistrationStatusReason)
        //        .FirstOrDefaultAsync(cancellationToken);

        //    var mainEconomicActivity = await economicActivitiesCollection
        //        .Find(c => c.Code == establishment.MainCnae)
        //        .FirstOrDefaultAsync(cancellationToken);

        //    var city = await citiesCollection
        //        .Find(c => c.Code == establishment.Address.MunicipalityCode)
        //        .FirstOrDefaultAsync(cancellationToken);

        //    var cityDto = new CityDto(int.Parse(city.Code), city.Description);

        //    var mainEconomicActivityDto = new EconomicActivityDto(int.Parse(mainEconomicActivity.Code), mainEconomicActivity.Description);

        //    var secondaryEconomicActivitiesCodes = establishment.SecondaryCnaes.Split(",");

        //    List<EconomicActivityDto> secondaryEconomicActivities = [];
        //    foreach (var secondaryEconomicActivitiesCode in secondaryEconomicActivitiesCodes)
        //    {
        //        if (string.IsNullOrEmpty(secondaryEconomicActivitiesCode))
        //            continue;

        //        var secondaryEconomicActivity = await economicActivitiesCollection
        //            .Find(c => c.Code == secondaryEconomicActivitiesCode)
        //            .FirstOrDefaultAsync(cancellationToken);

        //        secondaryEconomicActivities.Add(new EconomicActivityDto(int.Parse(secondaryEconomicActivity.Code), secondaryEconomicActivity.Description));
        //    }

        //    List<PhoneDto> contactPhones = [];
        //    if (!string.IsNullOrEmpty(establishment.Contact.PhoneAreaCode1) &&
        //        !string.IsNullOrEmpty(establishment.Contact.PhoneNumber1))
        //        contactPhones.Add(new PhoneDto(establishment.Contact.PhoneAreaCode1, establishment.Contact.PhoneNumber1));

        //    if (!string.IsNullOrEmpty(establishment.Contact.PhoneAreaCode2) &&
        //        !string.IsNullOrEmpty(establishment.Contact.PhoneNumber2))
        //        contactPhones.Add(new PhoneDto(establishment.Contact.PhoneAreaCode2, establishment.Contact.PhoneNumber2));

        //    establishmentDtos.Add(new EstablishmentDto(
        //        new CnpjDto(establishment.BasicCnpj, establishment.OrderCnpj, establishment.CheckDigitCnpj),
        //        CompanyTypeDto.CreateByID(establishment.HeadOfficeOrBranch),
        //        establishment.TradeName,
        //        CompanySituationDto.CreateByID(short.Parse(establishment.RegistrationStatus)),
        //        establishment.RegistrationStatusDate.Value,
        //        new ReasonDto(int.Parse(registrationStatusReason.Code), registrationStatusReason.Description),
        //        establishment.ForeignCityName,
        //        new AddressDto(cityDto, establishment.Address.StreetType, establishment.Address.StreetName, establishment.Address.Number, establishment.Address.District, establishment.Address.ZipCode, establishment.Address.State, establishment.Address.AdditionalAddressInfo),
        //        establishment.StartActivityDate.Value,
        //        mainEconomicActivityDto,
        //        secondaryEconomicActivities,
        //        new ContactDto(contactPhones, establishment.Contact.FaxAreaCode, establishment.Contact.FaxNumber, establishment.Contact.Email),
        //        null
        //        ));
        //}

        //var partners = await partnersCollection
        //        .Find(c => c.BasicCnpj == company.BasicCnpj)
        //        .ToListAsync(cancellationToken);

        //List<PartnerDto> partnerDtos = [];
        //foreach(var partner in partners)
        //{
        //    var partnerQualification = await partnersQualificationsCollection
        //        .Find(c => c.Code == partner.PartnerQualification)
        //        .FirstOrDefaultAsync(cancellationToken);

        //    var country = await countriesCollection
        //        .Find(c => c.Code == partner.CountryCode)
        //        .FirstOrDefaultAsync(cancellationToken);

        //    CountryDto? countryDto = null;
        //    if (country != null)
        //        countryDto = new(int.Parse(country.Code), country.Description);

        //    RepresentativeDto? representativeDto = null;
        //    if (!string.IsNullOrEmpty(partner.RepresentativeDocument) &&
        //        !string.IsNullOrEmpty(partner.RepresentativeName) &&
        //        !string.IsNullOrEmpty(partner.RepresentativeQualification))
        //    {
        //        var representativeQualification = await partnersQualificationsCollection
        //            .Find(c => c.Code == partner.PartnerQualification)
        //            .FirstOrDefaultAsync(cancellationToken);

        //        representativeDto = new(partner.RepresentativeDocument, partner.RepresentativeName, new PartnerQualificationDto(int.Parse(representativeQualification.Code), representativeQualification.Description));
        //    }

        //    partnerDtos.Add(new PartnerDto(PartnerTypeDto.CreateByID(partner.PartnerType), partner.PartnerName, partner.PartnerDocument, new PartnerQualificationDto(int.Parse(partnerQualification.Code), partnerQualification.Description), partner.EntryDate.Value, countryDto, representativeDto, AgeDto.CreateByID(short.Parse(partner.AgeRange))));
        //}

        //companyDtos.Add(new CompanyDto(new CnpjDto(company.BasicCnpj, null, null),
        //    company.CorporateName,
        //    legalNatureDto,
        //    mainPartnerQualification,
        //    company.ShareCapital,
        //    CompanySizeDto.CreateByID(company.CompanySize.Value),
        //    company.ResponsibleFederativeEntity, 
        //    simples.OptInSimple.Value, 
        //    simples.OptInMei.Value,
        //    partnerDtos,
        //    establishmentDtos
        //    ));

        return Result.Success<IEnumerable<CompanyDto>>(companyDtos);
    }
}
