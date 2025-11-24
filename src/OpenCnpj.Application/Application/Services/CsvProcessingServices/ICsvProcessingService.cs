using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.BatchFiles.Domain;
using System.Threading.Channels;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices;
public interface ICsvProcessingService
{
    Task<Result> ProcessCsvFilesIncremental(Batch batch, Channel<BatchFile> extractedFilesChannel, CancellationToken cancellationToken);
}