using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using System.Threading.Channels;

namespace OpenCnpj.Application.Application.Services.Interfaces;
public interface IFileExtractionService
{
    Task<Result> ExtractFilesIncremental(Batch batch, Channel<BatchFile> downloadedFilesChannel, Channel<BatchFile> extractedFilesChannel, CancellationToken cancellationToken);
}
