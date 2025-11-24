using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.BatchFiles.Domain;

namespace OpenCnpj.Application.Batches.Batches.Services;
public interface IBatchFileService
{
    Task<Result<BatchFile>> CreatePartialDownloadBatchFile(int batchID, string url, string filePath, CancellationToken cancellationToken);
    Task<Result<BatchFile>> CreateDownloadBatchFile(BatchFile parentBatchFile, string filePath, CancellationToken cancellationToken);
    Task<Result<BatchFile>> CreateExtractedBatchFile(BatchFile parentBatchFile, string filePath, CancellationToken cancellationToken);
    Task<Result> UpdateBatchFile(BatchFile batchFile, Func<Result> func, CancellationToken cancellationToken);
}