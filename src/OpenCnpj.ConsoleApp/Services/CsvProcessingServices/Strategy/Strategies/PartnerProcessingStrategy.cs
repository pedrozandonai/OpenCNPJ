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
public class PartnerProcessingStrategy(IMongoDatabaseFactory mongoDatabaseFactory, BatchSettings batchSettings, ILogger logger) : ICsvProcessingStrategy
{
    private readonly ILogger _logger = logger.ForContext<PartnerProcessingStrategy>();
    public string FilePattern => "Socios";

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<PartnerMapper>();
            var records = csvReader.GetRecords<PartnerRawRecord>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<PartnerRawRecord>(mongoDatabaseFactory, batchSettings, logger);
            await mongoDbBatchInsert.ProcessRecords(records, "PartnersRaw", cancellationToken);

            //await ProcessPartners(records, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }

    //private async Task ProcessPartners(IEnumerable<PartnerRawRecord> partners, CancellationToken cancellationToken)
    //{
    //    var collection = mongoDatabaseFactory
    //        .Database
    //        .GetCollection<PartnerRawRecord>("PartnersRaw");

    //    var buffer = new List<PartnerRawRecord>(batchSettings.Size);

    //    foreach (var record in partners)
    //    {
    //        buffer.Add(record);

    //        if (buffer.Count >= batchSettings.Size)
    //        {
    //            await collection.InsertManyAsync(buffer, cancellationToken: cancellationToken);
    //            buffer.Clear();
    //        }
    //    }

    //    if (buffer.Count > 0)
    //    {
    //        await collection.InsertManyAsync(buffer, cancellationToken: cancellationToken);
    //    }
    //}
}

