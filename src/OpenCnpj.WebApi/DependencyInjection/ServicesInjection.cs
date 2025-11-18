using OpenCnpj.Application.Application.Services;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Services;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class ServicesInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IBatchService, BatchService>();
        services.AddTransient<IFileExtractionService, FileExtractionService>();
        services.AddTransient<ICsvProcessingService, CsvProcessingService>();

        // *--Strategies for CSV Files--*
        services.AddScoped<CsvStrategyFactory>();
        services.AddScoped<CompanyProcessingStrategy>();
        services.AddScoped<EstablishmentProcessingStrategy>();
        services.AddScoped<PartnerProcessingStrategy>();
        services.AddScoped<SimpleDataProcessingStrategy>();

        return services;
    }
}
