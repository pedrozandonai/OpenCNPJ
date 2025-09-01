using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
using OpenCnpj.ConsoleApp.Application.RawRecords;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using OpenCnpj.ConsoleApp.Helpers;
using OpenCnpj.ConsoleApp.Mappers;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.ConsoleApp.Services.CsvProcessingServices.Strategy.Strategies;
public class ReferenceDataProcessingStrategy<T> : ICsvProcessingStrategy where T : RawRecordBase, new()
{
    private readonly IMongoDatabaseFactory _mongoDatabaseFactory;
    private readonly BatchSettings _batchSettings;
    private readonly ILogger _logger;
    private readonly string _filePattern;

    public string FilePattern => _filePattern;

    public ReferenceDataProcessingStrategy(string filePattern, IMongoDatabaseFactory mongoDatabaseFactory, BatchSettings batchSettings, ILogger logger)
    {
        _mongoDatabaseFactory = mongoDatabaseFactory;
        _batchSettings = batchSettings;
        _filePattern = filePattern;
        _logger = logger.ForContext<ReferenceDataProcessingStrategy<T>>();
    }

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap<RecordBaseMapper>();
            var records = csvReader.GetRecords<T>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<T>(_mongoDatabaseFactory, _batchSettings, _logger);
            await mongoDbBatchInsert.ProcessRecords(records, typeof(T).Name, cancellationToken);

            //await ProcessReferenceData(records, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }

    //private async Task ProcessReferenceData(IEnumerable<T> data, CancellationToken cancellationToken)
    //{
    //    var collection = _mongoDatabaseFactory
    //        .Database
    //        .GetCollection<T>(typeof(T).Name);

    //    var buffer = new List<T>(_batchSettings.Size);

    //    foreach (var record in data)
    //    {
    //        buffer.Add(record);

    //        if (buffer.Count >= _batchSettings.Size)
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
