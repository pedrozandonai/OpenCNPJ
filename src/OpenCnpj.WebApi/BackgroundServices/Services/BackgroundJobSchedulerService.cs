using Cronos;
using OpenCnpj.Core.Configurations;
using OpenCnpj.WebApi.BackgroundServices.Abstractions;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.WebApi.BackgroundServices.Services;

public class BackgroundJobSchedulerService(IServiceProvider serviceProvider, BackgroundSettings settings, ILogger logger) : BackgroundService
{
    private readonly ILogger _logger = logger.ForContext<BackgroundJobSchedulerService>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var jobTypes = scope.ServiceProvider.GetServices<BackgroundJob>();

        List<Task> tasks = [];

        foreach (var type in jobTypes)
            tasks.Add(StartJobLoop(type, stoppingToken));

        await Task.WhenAll(tasks);
    }

    private async Task StartJobLoop(BackgroundJob job, CancellationToken cancellationToken)
    {
        var jobName = job.GetType().Name.Replace("BackgroundService", "");

        if (!settings.BackgroundJobs.TryGetValue(jobName, out var jobSettings))
        {
            _logger.Warning("The background job {0} doesnt have any settings.", jobName);
            return;
        }

        if (!jobSettings.IsActive)
        {
            _logger.Information("The background job {0} is not active.", jobName);
            return;
        }

        var cronExpression = CronExpression.Parse(jobSettings.Cron, CronFormat.IncludeSeconds);

        while (!cancellationToken.IsCancellationRequested)
        {
            var nowUtc = DateTime.UtcNow;
            var next = cronExpression.GetNextOccurrence(nowUtc, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            if (!next.HasValue)
                continue;

            var delay = next.Value - nowUtc;

            if (delay.TotalMilliseconds > 0)
                await Task.Delay(delay, cancellationToken);

            try
            {
                await job.RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while executing background job {JobName}", jobName);
            }
        }
    }
}
