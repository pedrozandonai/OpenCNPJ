using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Queries;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class QueriesInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddTransient<IBatchQueries, BatchQueries>();

        return services;
    }
}
