using OpenCnpj.Core;
using System.Reflection;

namespace OpenCnpj.WebApi.DependencyInjection;

public static class MediatorInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] assemblyMarkerTypes)
    {
        services.AddScoped<IMediator, Mediator>();

        var assembly = Assembly.Load("OpenCnpj.Application");

        // Registrar Handlers de Request
        var requestHandlers = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in requestHandlers)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }

        // Registrar Handlers de Notification
        var notificationHandlers = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in notificationHandlers)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }

        return services;
    }
}
