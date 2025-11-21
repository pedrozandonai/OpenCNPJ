using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Mappers;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
public class PartnerProcessingStrategy(IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger) : ICsvProcessingStrategy
{
    private readonly ILogger _logger = logger.ForContext<PartnerProcessingStrategy>();
    public string FilePattern => "Socios";

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<PartnerMapper>();
            var records = csvReader.GetRecords<PartnersCollection>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<PartnersCollection>(mongoDatabaseFactory, tweakSettings, logger);
            await mongoDbBatchInsert.ProcessRecords(records, records.First().CollectionName, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }
}

