using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.DependencyInjection;
using OpenCnpj.ConsoleApp.HostedServices;

namespace OpenCnpj.ConsoleApp;

public static class Startup
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddConfigurations(configuration)
            .AddDatabase(configuration)
            .AddRepositories()
            .AddQueries()
            .AddClients()
            .AddServices();

        services.AddHostedService<OpenCnpjHostedService>();

        return services;
    }
}
