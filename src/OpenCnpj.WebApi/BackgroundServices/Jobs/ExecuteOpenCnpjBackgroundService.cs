using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Core.Configurations;
using OpenCnpj.WebApi.BackgroundServices.Abstractions;

namespace OpenCnpj.WebApi.BackgroundServices.Jobs;

public class ExecuteOpenCnpjBackgroundService(IOptions<BackgroundJobSettings> options, IServiceProvider serviceProvider, Serilog.ILogger logger) 
    : BackgroundJob(options.Value, serviceProvider, logger)
{
    protected override async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        var batchService = scope.ServiceProvider.GetRequiredService<IBatchService>();
        var openCnpjScrapperService = scope.ServiceProvider.GetRequiredService<IOpenCnpjScrapperService>();

        var batchPeriod = DateTime.Now.ToString("yyyy-MM");
        var batch = await batchService.GetOrCreateBatchByPeriod(batchPeriod, cancellationToken);
        if (batch.IsFailure)
            return Result.Failure(batch.Error);

        if (batch.Value.Operation == EBatchOperation.Finished)
        {
            batch = await batchService.CreateFutureBatch(cancellationToken);
            if (batch.IsFailure)
                return Result.Failure(batch.Error);
        }

        return await openCnpjScrapperService.ExecuteAsync(batch.Value, cancellationToken);
    }
}
