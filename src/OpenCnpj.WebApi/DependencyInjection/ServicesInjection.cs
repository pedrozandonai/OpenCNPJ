using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Application.Batches.BatchFiles.Services;
using OpenCnpj.WebApi.Services;
using OpenCnpj.WebApi.Services.CsvProcessingServices;
using OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.WebApi.Services.Interfaces;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class ServicesInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IBatchService, BatchService>();
        services.AddTransient<IFileExtractionService, FileExtractionService>();
        services.AddTransient<ICsvProcessingService, CsvProcessingService>();
        services.AddTransient<IBatchFileService, BatchFileService>();

        // *--Strategies for CSV Files--*
        services.AddScoped<CsvStrategyFactory>();
        services.AddScoped<CompanyProcessingStrategy>();
        services.AddScoped<EstablishmentProcessingStrategy>();
        services.AddScoped<PartnerProcessingStrategy>();
        services.AddScoped<SimpleDataProcessingStrategy>();

        return services;
    }
}
