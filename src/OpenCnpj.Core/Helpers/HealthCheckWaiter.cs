using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace OpenCnpj.Core.Helpers;

public static class HealthCheckWaiter
{
    public static async Task WaitForDatabasesAsync(IServiceProvider serviceProvider, int retries = 10, int delaySeconds = 5)
    {
        var logger = serviceProvider.GetRequiredService<ILogger>();
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        for (int i = 0; i < retries; i++)
        {
            var report = await healthCheckService.CheckHealthAsync();
            if (report.Status == HealthStatus.Healthy)
            {
                logger.Information("Databases are healthy.");
                return;
            }

            logger.Warning("Waiting for databases... Attempt {0}/{1}", i + 1, retries);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }

        throw new Exception("Databases not available after waiting.");
    }
}
