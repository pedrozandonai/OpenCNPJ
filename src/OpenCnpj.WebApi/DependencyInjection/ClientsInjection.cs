using OpenCnpj.WebApi.Clients;
using OpenCnpj.WebApi.Clients.Interfaces;

namespace OpenCnpj.WebApi.DependencyInjection;
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
