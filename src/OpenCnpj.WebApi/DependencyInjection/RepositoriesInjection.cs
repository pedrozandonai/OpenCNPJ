using OpenCnpj.Application.Batches.Batches.Repositories;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class RepositoriesInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBatchRepository, BatchRepository>();

        return services;
    }
}
