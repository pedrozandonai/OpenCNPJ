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
internal class SimpleDataProcessingStrategy(IMongoDatabaseFactory mongoDatabaseFactory, BatchSettings batchSettings, ILogger logger) : ICsvProcessingStrategy
{
    private readonly ILogger _logger = logger.ForContext<SimpleDataProcessingStrategy>();
    public string FilePattern => "Simples";

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<SimpleDataMapper>();
            var records = csvReader.GetRecords<SimpleDataRawRecord>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<SimpleDataRawRecord>(mongoDatabaseFactory, batchSettings, logger);
            await mongoDbBatchInsert.ProcessRecords(records, "SimplesDataRaw", cancellationToken);

            //await ProcessSimpleData(records, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }

    //private async Task ProcessSimpleData(IEnumerable<SimpleDataRawRecord> simplesData, CancellationToken cancellationToken)
    //{
    //    var collection = mongoDatabaseFactory
    //        .Database
    //        .GetCollection<SimpleDataRawRecord>("SimplesDataRaw");

    //    var buffer = new List<SimpleDataRawRecord>(batchSettings.Size);

    //    foreach (var record in simplesData)
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
