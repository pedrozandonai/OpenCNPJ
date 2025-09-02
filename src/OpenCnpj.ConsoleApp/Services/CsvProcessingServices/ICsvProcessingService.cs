using CSharpFunctionalExtensions;
using CsvHelper.Configuration;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Services.CsvProcessingServices;
public interface ICsvProcessingService
{
    Task<Result> ProcessCsvFiles(Batch batch, CancellationToken cancellationToken);
    Task<Result> ProcessSingleFile(Batch batch, string filePath, CsvConfiguration config, CancellationToken cancellationToken);
}