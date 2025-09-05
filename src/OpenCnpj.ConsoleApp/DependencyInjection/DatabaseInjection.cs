using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Database.Helpers;
using OpenCnpj.Core.Database.Migrations;

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

        services.AddScoped<IDatabaseFactory>(sr => new DatabaseFactory(postgresConnectionString));

        var mongoDatabaseFactory = new MongoDatabaseFactory(mongoConnectionString, "OpenCnpj");
        services.AddSingleton<IMongoDatabaseFactory>(sr => mongoDatabaseFactory);

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Postgresql")!,
               name: "PostgreSQL Health Check",
               failureStatus: HealthStatus.Unhealthy);

        services.AddHealthChecks()
        .AddMongoDb(sr => mongoDatabaseFactory.Client,
            failureStatus: HealthStatus.Unhealthy);

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
            .AddPostgres()
            .WithGlobalConnectionString(postgresConnectionString)
            .ScanIn(typeof(InitialMigration).Assembly).For.All())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }

    public static async Task UpdateDatabase(IServiceProvider serviceProvider)
    {
        var databaseSettings = serviceProvider.GetRequiredService<DatabaseSettings>();
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        var mongoFactory = serviceProvider.GetRequiredService<IMongoDatabaseFactory>();

        if (databaseSettings.FormatMongoDB!.Value)
        {
            var client = mongoFactory.Client;
            var databaseName = mongoFactory.Database.DatabaseNamespace.DatabaseName;

            await client.DropDatabaseAsync(databaseName);
        }

        await MongoIndexes.EnsureIndexes(mongoFactory);

        if (databaseSettings.FormatPostgres!.Value)
            await SqlDatabaseHelper.DropSchema(serviceProvider);

        runner.MigrateUp();
    }
}
