using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Application.Addresses.Services;
using OpenCnpj.ConsoleApp.Application.AddressTypes.Services;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Services;
using OpenCnpj.ConsoleApp.Application.Batches.BatchFiles.Services;
using OpenCnpj.ConsoleApp.Application.Cities.Services;
using OpenCnpj.ConsoleApp.Application.Companies.Services;
using OpenCnpj.ConsoleApp.Application.Countries.Services;
using OpenCnpj.ConsoleApp.Application.EconomicActivities.Services;
using OpenCnpj.ConsoleApp.Application.LegalNatures.Services;
using OpenCnpj.ConsoleApp.Application.PartnersQualifications.Services;
using OpenCnpj.ConsoleApp.Application.Reasons.Services;
using OpenCnpj.ConsoleApp.Services;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.ConsoleApp.Services.Interfaces;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class ServicesInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IBatchService, BatchService>();
        services.AddTransient<IFileExtractionService, FileExtractionService>();
        services.AddTransient<ICsvProcessingService, CsvProcessingService>();
        services.AddTransient<IFormatDataService, FormatDataService>();
        services.AddTransient<ICityService, CityService>();
        services.AddTransient<IEconomicActivityService, EconomicActivityService>();
        services.AddTransient<ICountryServices, CountryServices>();
        services.AddTransient<ILegalNatureService, LegalNatureService>();
        services.AddTransient<IPartnerQualificationService, PartnerQualificationService>();
        services.AddTransient<IReasonService, ReasonService>();
        services.AddTransient<IBatchFileService, BatchFileService>();
        services.AddTransient<IAddressTypeService, AddressTypeService>();
        services.AddTransient<IAddressService, AddressService>();
        services.AddTransient<ICompanyService, CompanyService>();

        // *--Strategies for CSV Files--*
        services.AddScoped<CsvStrategyFactory>();
        services.AddScoped<CompanyProcessingStrategy>();
        services.AddScoped<EstablishmentProcessingStrategy>();
        services.AddScoped<PartnerProcessingStrategy>();
        services.AddScoped<SimpleDataProcessingStrategy>();

        return services;
    }
}
