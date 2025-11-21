using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using System.Threading.Channels;

namespace OpenCnpj.Application.Government.Clients.Interfaces;

public interface IGovernmentHttpClient
{
    Task<Result> DownloadCurrentBatch(
        Batch batch,
        Channel<BatchFile> downloadedFilesChannel,
        CancellationToken cancellationToken);
}
