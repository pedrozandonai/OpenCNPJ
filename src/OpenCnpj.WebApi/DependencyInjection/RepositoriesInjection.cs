using OpenCnpj.Application.Batches.Batches.Repositories;
using OpenCnpj.Application.Batches.BatchFiles.Repositories;

namespace OpenCnpj.WebApi.DependencyInjection;
public static class RepositoriesInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBatchRepository, BatchRepository>();
        services.AddTransient<IBatchFileRepository, BatchFileRepository>();

        return services;
    }
}
