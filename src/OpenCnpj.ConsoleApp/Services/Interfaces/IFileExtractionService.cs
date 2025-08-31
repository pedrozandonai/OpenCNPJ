using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Services.Interfaces;
public interface IFileExtractionService
{
    Task<Result> ExtractFiles(Batch batch, CancellationToken cancellationToken);
}
