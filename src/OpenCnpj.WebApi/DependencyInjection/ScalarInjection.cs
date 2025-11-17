using Scalar.AspNetCore;

namespace OpenCnpj.WebApi.DependencyInjection;

public static class ScalarInjection
{
    public static IServiceCollection AddScalar(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new()
                {
                    Title = "OpenCNPJ",
                    Version = "v1",
                    Description = ""
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IEndpointRouteBuilder MapScalarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapScalarApiReference(opt =>
        {
            opt.Title = "OpenCNPJ";
            opt.Theme = ScalarTheme.BluePlanet;
            opt.DarkMode = true;
            opt.Servers = [];
        });

        return app;
    }
}