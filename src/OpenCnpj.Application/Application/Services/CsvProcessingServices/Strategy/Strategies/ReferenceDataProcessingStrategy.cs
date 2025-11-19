using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.Application.Application.Mappers;
using OpenCnpj.Application.Batches.Domain;
using OpenCnpj.Application.MongoApplicationCollections.Collections;
using OpenCnpj.Application.MongoApplicationCollections.Domain;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
public class ReferenceDataProcessingStrategy<T> : ICsvProcessingStrategy where T : CollectionBase, IMongoApplicationCollection
{
    private readonly IMongoDatabaseFactory _mongoDatabaseFactory;
    private readonly TweakSettings _tweakSettings;
    private readonly ILogger _logger;
    private readonly string _filePattern;

    public string FilePattern => _filePattern;

    public ReferenceDataProcessingStrategy(string filePattern, IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger)
    {
        _mongoDatabaseFactory = mongoDatabaseFactory;
        _tweakSettings = tweakSettings;
        _filePattern = filePattern;
        _logger = logger.ForContext<ReferenceDataProcessingStrategy<T>>();
    }

    public async Task<Result> ProcessAsync(Batch batch, CsvReader csvReader, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            csvReader.Context.RegisterClassMap(new RawRecordBaseMap<T>());
            var records = csvReader.GetRecords<T>().ToList();

            var mongoDbBatchInsert = new MongoDbBatchInsert<T>(_mongoDatabaseFactory, _tweakSettings, _logger);
            await mongoDbBatchInsert.ProcessRecords(records, records[0].CollectionName, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }
}
