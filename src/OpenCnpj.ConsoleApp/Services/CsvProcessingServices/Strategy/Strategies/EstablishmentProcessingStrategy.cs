using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using OpenCnpj.ConsoleApp.Helpers;
using OpenCnpj.ConsoleApp.Mappers;
using Serilog;

namespace OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Strategies;
public class EstablishmentProcessingStrategy(IMongoDatabaseFactory mongoDatabaseFactory, BatchSettings batchSettings, ILogger logger) : ICsvProcessingStrategy
{
    private readonly ILogger _logger = logger.ForContext<EstablishmentProcessingStrategy>();
    public string FilePattern => "Estabelecimento";

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<EstablishmentMapper>();
            var records = csvReader.GetRecords<EstablishmentRawRecord>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<EstablishmentRawRecord>(mongoDatabaseFactory, batchSettings, logger);
            await mongoDbBatchInsert.ProcessRecords(records, "EstablishmentsRaw", cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }
}
