using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Application.Addresses.Domain;
using OpenCnpj.Application.AddressTypes.Domain;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Cities.Domain;
using OpenCnpj.Application.Companies.Domain;
using OpenCnpj.Application.CompanySpecialSituations.Domain;
using OpenCnpj.Application.Countries.Domain;
using OpenCnpj.Application.EconomicActivities.Domain;
using OpenCnpj.Application.LegalNatures.Domain;
using OpenCnpj.Application.PartnersQualifications.Domain;
using OpenCnpj.Application.Reasons.Domain;
using OpenCnpj.Application.SpecialSituations.Domain;
using OpenCnpj.Infraestructure.Repositories;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class RepositoriesInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBatchRepository, BatchRepository>();
        services.AddTransient<ICityRepository, CityRepository>();
        services.AddTransient<IEconomicActivityRepository, EconomicActivityRepository>();
        services.AddTransient<ICountryRepository, CountryRepository>();
        services.AddTransient<ILegalNatureRepository, LegalNatureRepository>();
        services.AddTransient<IPartnerQualificationRepository, PartnerQualificationRepository>();
        services.AddTransient<IReasonRepository, ReasonRepository>();
        services.AddTransient<ICompanyRepository, CompanyRepository>();
        services.AddTransient<IAddressTypeRepository, AddressTypeRepository>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<ISpecialSituationRepository, SpecialSituationRepository>();
        services.AddTransient<IEconomicActivityRepository, EconomicActivityRepository>();
        services.AddTransient<ICompanySpecialSituationRepository, CompanySpecialSituationRepository>();

        return services;
    }
}
