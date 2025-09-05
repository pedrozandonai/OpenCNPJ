using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Clients.Interfaces;

public interface IGovernmentHttpClient
{
    Task<Result> DownloadCurrentBatch(Batch batch, CancellationToken cancellationToken);
}
