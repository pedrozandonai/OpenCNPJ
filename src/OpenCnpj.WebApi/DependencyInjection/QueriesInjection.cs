using OpenCnpj.Application.Batches.Batches.Queries;
using OpenCnpj.Application.Batches.BatchFiles.Queries;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class QueriesInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddTransient<IBatchQueries, BatchQueries>();
        services.AddTransient<IBatchFileQueries, BatchFileQueries>();

        return services;
    }
}
