using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenCnpj.ConsoleApp.DependencyInjection;
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

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Application Started!");

            var host = CreateHost(configuration);

            RunMigrations(host.Services);

            await host.RunAsync();

            Log.Information("The application has finished.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while the application was running.");
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

    private static void RunMigrations(IServiceProvider serviceProvider)
    {
        DatabaseInjection.UpdateDatabase(serviceProvider);
    }

}