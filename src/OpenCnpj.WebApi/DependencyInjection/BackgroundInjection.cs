using OpenCnpj.Core.Configurations;
using OpenCnpj.WebApi.BackgroundServices.Abstractions;
using OpenCnpj.WebApi.BackgroundServices.Services;

namespace OpenCnpj.WebApi.DependencyInjection;

public static class BackgroundInjection
{
    public static IServiceCollection AddBackgrounds(this IServiceCollection services, IConfiguration configuration)
    {
        BackgroundSettings backgroundSettings = new();
        configuration.GetRequiredSection(nameof(BackgroundSettings)).Bind(backgroundSettings);
        services.AddSingleton(backgroundSettings);
        
        services.AddHostedService<BackgroundJobSchedulerService>();

        var jobs = typeof(BackgroundJob).Assembly.DefinedTypes
            .Where(t => !t.IsAbstract && typeof(BackgroundJob).IsAssignableFrom(t));

        foreach (var job in jobs)
            services.AddScoped(typeof(BackgroundJob), job);

        return services;
    }
}
