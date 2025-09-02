using Dapper;
using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Core.Database.Helpers;
public static class SqlDatabaseHelper
{
    public static async Task DropSchema(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetRequiredService<ILogger>();

        try
        {
            var databaseFactory = serviceProvider.GetRequiredService<IDatabaseFactory>();

            const string sql = @"drop schema if exists public cascade; create schema public";

            var command = new CommandDefinition(sql, transaction: databaseFactory.Transaction, cancellationToken: cancellationToken);

            await databaseFactory.Connection.ExecuteAsync(command);
        }
        catch(Exception ex)
        {
            logger.Error(ex, "An error occured while trying to drop the database schema.");
        }
    }
}
