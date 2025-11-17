using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using OpenCnpj.WebApi.Mappers;
using OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.WebApi.Services.CsvProcessingServices.Strategy.Strategies;
internal class SimpleDataProcessingStrategy(IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger) : ICsvProcessingStrategy
{
    private readonly ILogger _logger = logger.ForContext<SimpleDataProcessingStrategy>();
    public string FilePattern => "Simples";

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<SimpleDataMapper>();
            var records = csvReader.GetRecords<SimpleDataRawRecord>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<SimpleDataRawRecord>(mongoDatabaseFactory, tweakSettings, logger);
            await mongoDbBatchInsert.ProcessRecords(records, "SimplesDataRaw", cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }
}
