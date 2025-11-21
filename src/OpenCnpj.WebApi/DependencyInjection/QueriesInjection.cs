using OpenCnpj.Application.Batches.Batches.Queries;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class QueriesInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddTransient<IBatchQueries, BatchQueries>();

        return services;
    }
}
