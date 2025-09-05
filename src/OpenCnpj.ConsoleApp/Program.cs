using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenCnpj.ConsoleApp.DependencyInjection;
using OpenCnpj.Core.Helpers;
using Serilog;

namespace OpenCnpj.ConsoleApp;

internal static class Program
{
    internal static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateBootstrapLogger();

        try
        {
            logger.Information("Application Started!");

            var host = CreateHost(configuration);

            await RunMigrations(host.Services);

            await host.RunAsync();

            logger.Information("The application has finished.");
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "An error occurred while the application was running.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHost CreateHost(IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder()
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration);
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices(configuration);
            })
            .Build();
    }

    private static async Task RunMigrations(IServiceProvider serviceProvider)
    {
        // Aguarda os bancos de dados subirem antes de executar os migrations caso a execução tenha se dado pelo docker composer.
        await HealthCheckWaiter.WaitForDatabasesAsync(serviceProvider);

        await DatabaseInjection.UpdateDatabase(serviceProvider);
    }
}