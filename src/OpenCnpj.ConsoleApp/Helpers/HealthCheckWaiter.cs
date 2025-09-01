using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OpenCnpj.ConsoleApp.Helpers;

public static class HealthCheckWaiter
{
    public static async Task WaitForDatabasesAsync(IServiceProvider serviceProvider, int retries = 10, int delaySeconds = 5)
    {
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        for (int i = 0; i < retries; i++)
        {
            var report = await healthCheckService.CheckHealthAsync();
            if (report.Status == HealthStatus.Healthy)
            {
                Console.WriteLine("Databases are healthy.");
                return;
            }

            Console.WriteLine($"Waiting for databases... Attempt {i + 1}/{retries}");
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }

        throw new Exception("Databases not available after waiting.");
    }
}
