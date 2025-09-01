using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using OpenCnpj.ConsoleApp.Core.Database.Migrations;
using OpenCnpj.ConsoleApp.Helpers;

namespace OpenCnpj.ConsoleApp.DependencyInjection;

public static class DatabaseInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Postgresql");
        if (string.IsNullOrEmpty(postgresConnectionString))
            throw new Exception("The 'Postgresql' connection string can not be null or empty.");

        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        if (string.IsNullOrEmpty(mongoConnectionString))
            throw new Exception("The 'MongoDB' connection string can not be null or empty.");

        services.AddScoped<IDatabaseFactory>(_ => new DatabaseFactory(postgresConnectionString));

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Postgresql")!,
               name: "PostgreSQL Health Check",
               failureStatus: HealthStatus.Unhealthy);

        var mongoDatabaseFactory = new MongoDatabaseFactory(mongoConnectionString, "OpenCnpj");
        services.AddSingleton<IMongoDatabaseFactory>(_ => mongoDatabaseFactory);

        services.AddHealthChecks()
        .AddMongoDb(sr => mongoDatabaseFactory.Client,
            failureStatus: HealthStatus.Unhealthy);

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
            //.AddDb2() kkkkkk
            .AddPostgres()
            .WithGlobalConnectionString(postgresConnectionString)
            .ScanIn(typeof(InitialMigration).Assembly).For.All())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }

    public static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var databaseSettings = serviceProvider.GetRequiredService<DatabaseSettings>();
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        var initialMigration = new InitialMigration();

        if (databaseSettings.FormatMongoDB!.Value)
        {
            var mongoFactory = serviceProvider.GetRequiredService<IMongoDatabaseFactory>();
            var client = mongoFactory.Client;
            var databaseName = mongoFactory.Database.DatabaseNamespace.DatabaseName;

            // Remove o database inteiro
            client.DropDatabase(databaseName);
        }

        if (databaseSettings.FormatPostgres!.Value)
        {
            //runner.Down(initialMigration);
            //runner.Up(initialMigration);
            runner.MigrateUp();
        }
    }
}
