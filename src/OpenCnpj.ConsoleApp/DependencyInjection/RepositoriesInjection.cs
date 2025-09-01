using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Repositories;
using OpenCnpj.ConsoleApp.Application.Cities.Repositories;
using OpenCnpj.ConsoleApp.Application.Companies.Repositories;
using OpenCnpj.ConsoleApp.Application.Countries.Repositories;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Repositories;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Repositories;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Repositories;
using OpenCnpj.ConsoleApp.Application.Reasons.Repositories;

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

        return services;
    }
}
