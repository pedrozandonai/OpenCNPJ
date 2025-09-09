using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Application.Addresses.Services;
using OpenCnpj.Application.AddressTypes.Services;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Services;
using OpenCnpj.Application.Cities.Services;
using OpenCnpj.Application.Companies.Services;
using OpenCnpj.Application.Countries.Services;
using OpenCnpj.Application.EconomicActivities.Services;
using OpenCnpj.Application.LegalNatures.Services;
using OpenCnpj.Application.PartnersQualifications.Services;
using OpenCnpj.Application.Reasons.Services;
using OpenCnpj.Application.Simples.Services;
using OpenCnpj.ConsoleApp.Services;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.ConsoleApp.Services.Interfaces;
using OpenCnpj.Core.Database.Services;

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
        services.AddTransient<ISimpleService, SimpleService>();
        services.AddSingleton<IPgBulkCopyService, PgBulkCopyService>();

        // *--Strategies for CSV Files--*
        services.AddScoped<CsvStrategyFactory>();
        services.AddScoped<CompanyProcessingStrategy>();
        services.AddScoped<EstablishmentProcessingStrategy>();
        services.AddScoped<PartnerProcessingStrategy>();
        services.AddScoped<SimpleDataProcessingStrategy>();

        return services;
    }
}
