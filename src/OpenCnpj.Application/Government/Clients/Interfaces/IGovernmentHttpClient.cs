using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Government.Clients.Interfaces;

public interface IGovernmentHttpClient
{
    Task<Result> DownloadCurrentBatch(Batch batch, CancellationToken cancellationToken);
}
