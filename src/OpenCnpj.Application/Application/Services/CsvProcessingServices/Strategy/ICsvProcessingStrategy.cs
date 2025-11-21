using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.Application.Batches.Batches.Domain;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy;
public interface ICsvProcessingStrategy
{
    string FilePattern { get; }
    Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken);
}
