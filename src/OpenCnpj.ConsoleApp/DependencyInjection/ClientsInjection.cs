using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Clients;
using OpenCnpj.ConsoleApp.Clients.Interfaces;

namespace OpenCnpj.ConsoleApp.DependencyInjection;
public static class ClientsInjection
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient<IGovernmentHttpClient, GovernmentHttpClient>(client =>
        {
            client.Timeout = Timeout.InfiniteTimeSpan;
        });

        return services;
    }
}
