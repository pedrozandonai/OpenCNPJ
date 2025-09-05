using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenCnpj.Infraestructure.DbContexts;

namespace OpenCnpj.Infraestructure.Helpers;
public static class SqlDatabaseHelper
{
    public static async Task DropSchema(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OpenCnpjDbContext>();

        var schema = dbContext.Model.GetDefaultSchema() ?? "public";

        // apaga o schema inteiro
        await dbContext.Database.ExecuteSqlRawAsync($@"drop schema if exists {schema} cascade;create schema {schema};");
    }
}
