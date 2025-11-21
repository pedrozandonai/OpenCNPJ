using CSharpFunctionalExtensions;
using OpenCnpj.Application.Application.Commands;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Application.Handlers;
public class PostManualApplicationScrapperCommandHandler(IBatchFileService batchService, IOpenCnpjScrapperService openCnpjScrapperService) : IRequestHandler<PostManualApplicationScrapperCommand, Result>
{
    public async Task<Result> Handle(PostManualApplicationScrapperCommand request, CancellationToken cancellationToken)
    {
        var batchIdentifier = DateTime.Now.ToString("yyyy-MM");
        if (request.BatchMonth.HasValue)
        {
            if (request.BatchMonth.Value is < 1 or > 12)
                return Result.Failure("Please insert a valid batch month between 1 and 12.");

            if (!request.BatchYear.HasValue || request.BatchYear.Value <= 0)
                return Result.Failure("Please insert a valid batch year.");

            batchIdentifier = string.Format("{0}-{1}", request.BatchYear.Value, request.BatchMonth.Value);
        }

        var batch = await batchService.GetOrCreateBatchByIdentifier(batchIdentifier, cancellationToken);
        if (batch.IsFailure)
            return batch;

        return await openCnpjScrapperService.ExecuteAsync(batch.Value, cancellationToken);
    }
}
