using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Helpers;
public class MongoDbBatchInsert<T>
{
    private readonly IMongoDatabaseFactory _mongoDatabaseFactory;
    private readonly TweakSettings _tweakSettings;
    private readonly ILogger _logger;

    public MongoDbBatchInsert(IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger)
    {
        _mongoDatabaseFactory=mongoDatabaseFactory;
        _tweakSettings=tweakSettings;
        _logger=logger;
    }

    public async Task ProcessRecords(IEnumerable<T> records, string collectionName, CancellationToken cancellationToken)
    {
        var collection = _mongoDatabaseFactory
            .Database
            .GetCollection<T>(collectionName);

        var buffer = new List<T>(_tweakSettings.RawFilesProcessingSettings.RecordsBatchAmount);

        foreach (var record in records)
        {
            buffer.Add(record);

            if (buffer.Count >= _tweakSettings.RawFilesProcessingSettings.RecordsBatchAmount)
            {
                await InsertBatchOptimized(collection, buffer, cancellationToken);
                buffer.Clear();
            }
        }

        if (buffer.Count > 0)
        {
            await InsertBatchOptimized(collection, buffer, cancellationToken);
        }
    }

    private async Task InsertBatchOptimized(IMongoCollection<T> collection,
    List<T> records, CancellationToken cancellationToken)
    {
        try
        {
            await collection.InsertManyAsync(records,
                new InsertManyOptions
                {
                    IsOrdered = false,
                    BypassDocumentValidation = false
                },
                cancellationToken: cancellationToken);

            _logger.Debug("Successfully inserted {Count} records", records.Count);
        }
        catch (MongoBulkWriteException ex)
        {
            var otherErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();

            if (otherErrors.Count != 0)
            {
                _logger.Error(ex, "Critical errors during bulk insert: {@Errors}",
                    otherErrors.Select(e => new { e.Code, e.Message }));
                throw; 
            }
        }
    }
}
