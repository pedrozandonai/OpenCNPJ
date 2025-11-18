using FluentMigrator.Runner;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Database.Helpers;
using OpenCnpj.Core.Database.Migrations;

namespace OpenCnpj.WebApi.DependencyInjection;

public static class DatabaseInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        if (string.IsNullOrEmpty(mongoConnectionString))
            throw new Exception("The 'MongoDB' connection string can not be null or empty.");

        SqliteHelper.InitializeSqliteFolder();

        var sqliteDatabaseFactory = new DatabaseFactory(SqliteHelper.GetSqliteConnectionString());
        services.AddScoped<IDatabaseFactory>(sr => sqliteDatabaseFactory);

        var mongoDatabaseFactory = new MongoDatabaseFactory(mongoConnectionString, "OpenCnpj");
        services.AddScoped<IMongoDatabaseFactory>(sr => mongoDatabaseFactory);

        services.AddHealthChecks()
        .AddMongoDb(sr => mongoDatabaseFactory.Client,
            failureStatus: HealthStatus.Unhealthy);

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
            .AddSQLite()
            .WithGlobalConnectionString(SqliteHelper.GetSqliteConnectionString())
            .ScanIn(typeof(InitialMigration).Assembly).For.All())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }

    public static IApplicationBuilder UseDatabase(this IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();

        var databaseSettings = scope.ServiceProvider.GetRequiredService<DatabaseSettings>();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var mongoFactory = scope.ServiceProvider.GetRequiredService<IMongoDatabaseFactory>();

        if (databaseSettings.FormatMongoDB!.Value)
        {
            var client = mongoFactory.Client;
            var databaseName = mongoFactory.Database.DatabaseNamespace.DatabaseName;

            Task.Run(async () => await client.DropDatabaseAsync(databaseName));
        }

        Task.Run(async () => await MongoIndexes.EnsureIndexes(mongoFactory));

        runner.MigrateUp();

        return applicationBuilder;
    }
}
