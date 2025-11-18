using CSharpFunctionalExtensions;
using CsvHelper;
using OpenCnpj.Application.Application.Mappers;
using OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using ILogger = Serilog.ILogger;

namespace OpenCnpj.Application.Application.Services.CsvProcessingServices.Strategy.Strategies;
public class ReferenceDataProcessingStrategy<T> : ICsvProcessingStrategy where T : RawRecordBase, new()
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
            csvReader.Context.RegisterClassMap<RecordBaseMapper>();
            var records = csvReader.GetRecords<T>();

            var mongoDbBatchInsert = new MongoDbBatchInsert<T>(_mongoDatabaseFactory, _tweakSettings, _logger);
            await mongoDbBatchInsert.ProcessRecords(records, typeof(T).Name, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while processing records of the file: {0}", fileName);
            return Result.Failure($"Error while processing records: {ex.Message}");
        }
    }
}
