using MongoDB.Driver;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.Core.Helpers;
public class MongoDbBatchInsert<T>(IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger)
{
    public async Task ProcessRecords(IEnumerable<T> records, string collectionName, CancellationToken cancellationToken)
    {
        string tempCollectionName = string.Format("{0}_temp", collectionName); // Adiciona _temp no nome da coleção do mongo pra quando atualizar os registros, deletar todos existentes e trocar os nomes definitivamente.

        var collection = mongoDatabaseFactory
            .Database
            .GetCollection<T>(tempCollectionName);

        // cria índices para T na collection temp
        await MongoIndexHelper.EnsureIndexesForType(collection, cancellationToken);

        var buffer = new List<T>(tweakSettings.RawFilesProcessingSettings.RecordsBatchAmount);

        foreach (var record in records)
        {
            buffer.Add(record);

            if (buffer.Count >= tweakSettings.RawFilesProcessingSettings.RecordsBatchAmount)
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

            logger.Debug("Successfully inserted {Count} records", records.Count);
        }
        catch (MongoBulkWriteException ex)
        {
            var otherErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();

            if (otherErrors.Count != 0)
            {
                logger.Error(ex, "Critical errors during bulk insert: {@Errors}",
                    otherErrors.Select(e => new { e.Code, e.Message }));
                throw; 
            }
        }
    }
}
