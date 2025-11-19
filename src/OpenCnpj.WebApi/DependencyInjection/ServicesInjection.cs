using OpenCnpj.Application.Application.Services;
using OpenCnpj.Application.Application.Services.CsvProcessingServices;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Factory;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Services;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class ServicesInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IBatchService, BatchService>();
        services.AddTransient<IFileExtractionService, FileExtractionService>();
        services.AddTransient<ICsvProcessingService, CsvProcessingService>();
        services.AddTransient<IOpenCnpjScrapperService, OpenCnpjScrapperService>();
        services.AddTransient<IMongoCollectionsService, MongoCollectionsService>();

        // *--Strategies for CSV Files--*
        services.AddTransient<CsvStrategyFactory>();
        services.AddTransient<CompanyProcessingStrategy>();
        services.AddTransient<EstablishmentProcessingStrategy>();
        services.AddTransient<PartnerProcessingStrategy>();
        services.AddTransient<SimpleDataProcessingStrategy>();

        return services;
    }
}
