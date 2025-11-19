using Asp.Versioning;
using OpenCnpj.Core.Database.SqlMappers;
using OpenCnpj.WebApi.DependencyInjection;

namespace OpenCnpj.WebApi;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddConfigurations(configuration)
            .AddBackgrounds(configuration)
            .AddDatabase(configuration)
            .AddMediator()
            .AddRepositories()
            .AddQueries()
            .AddClients()
            .AddServices();

        SqlMappers.AddSqlMappers();

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddHealthChecks();
        services.AddControllers();
        services.AddScalar();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
                endpoints.MapScalarEndpoints();
            }
        });

        app.UseDatabase();
    }
}
