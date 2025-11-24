using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Commands;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Models.Enums;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Application.Handlers;
public class PostManualApplicationScrapperCommandHandler(IBatchService batchService, IOpenCnpjScrapperService openCnpjScrapperService) : IRequestHandler<PostManualApplicationScrapperCommand, Result>
{
    public async Task<Result> Handle(PostManualApplicationScrapperCommand request, CancellationToken cancellationToken)
    {
        var batchPeriod = DateTime.Now.ToString("yyyy-MM");
        if (request.BatchMonth.HasValue)
        {
            if (request.BatchMonth.Value is < 1 or > 12)
                return Result.Failure("Please insert a valid batch month between 1 and 12.");

            if (!request.BatchYear.HasValue || request.BatchYear.Value <= 0)
                return Result.Failure("Please insert a valid batch year.");

            batchPeriod = string.Format("{0}-{1}", request.BatchYear.Value, request.BatchMonth.Value);
        }

        var batch = await batchService.GetOrCreateBatchByPeriod(batchPeriod, cancellationToken);
        if (batch.IsFailure)
            return batch;

        if (batch.Value.Operation == EBatchOperation.Finished)
            return Result.Failure("The informed batch is already finished.");

        return await openCnpjScrapperService.ExecuteAsync(batch.Value, cancellationToken);
    }
}
