using CSharpFunctionalExtensions;
using OpenCnpj.Core.Configurations;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.WebApi.BackgroundServices.Abstractions;

public abstract class BackgroundJob
{
    protected readonly BackgroundJobSettings _backgroundJobSettings;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ILogger _logger;
    private readonly string _jobContext;

    protected BackgroundJob(BackgroundJobSettings backgroundJobSettings, IServiceProvider serviceProvider, ILogger logger)
    {
        _backgroundJobSettings = backgroundJobSettings;
        _serviceProvider = serviceProvider;
        _logger = logger.ForContext<BackgroundJob>();
        _jobContext = string.Format("[{0}]", GetType().Name);
    }

    protected abstract Task<Result> ExecuteAsync(CancellationToken cancellationToken);

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();

        Exception? finalException = null;

        for (int attempts = 0; attempts < _backgroundJobSettings.RetryAmount; attempts++)
        {
            try
            {
                _logger.Debug("{0} Starting background job executiong...", _jobContext);
                var executionResult = await ExecuteAsync(cancellationToken);

                if (executionResult.IsSuccess)
                    break;

                _logger.Error("{0} The background job failed... Retry {1} of {2}. Error: {3}", _jobContext, attempts, _backgroundJobSettings.RetryAmount, executionResult.Error);

                if (attempts >= _backgroundJobSettings.RetryAmount)
                {
                    finalException = new Exception(string.Format("The background job failed after {0} attempts. Last exception error: {1}", attempts, executionResult.Error));
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(_backgroundJobSettings.RetryDelay), cancellationToken);
            }
            catch (Exception ex)
            {
                finalException = ex;
                break;
            }
        }

        if (finalException is not null)
            _logger!.Error(finalException, "{0}", _jobContext);

        _logger.Debug("{0} Background job finished!", _jobContext);
    }
}
