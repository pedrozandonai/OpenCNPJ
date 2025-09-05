using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Core.Database.Helpers;
public static class DbScopeHelper
{
    public static async Task ExecuteInNewScopeAsync(
        Func<IDatabaseFactory, Task> action,
        Func<IDatabaseFactory> factoryProvider,
        CancellationToken cancellationToken)
    {
        var databaseFactory = factoryProvider();

        try
        {
            await databaseFactory.BeginAsync();

            await action(databaseFactory);

            await databaseFactory.CommitAsync();
        }
        catch
        {
            await databaseFactory.RollbackAsync();
            throw;
        }
    }
}

