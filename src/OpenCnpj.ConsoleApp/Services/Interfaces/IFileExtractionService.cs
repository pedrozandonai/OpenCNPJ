using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Services.Interfaces;
public interface IFileExtractionService
{
    Task<Result> ExtractFiles(Batch batch, CancellationToken cancellationToken);
}
