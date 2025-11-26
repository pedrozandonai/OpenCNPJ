using MongoDB.Driver;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.Core.Helpers;

public class MongoDbBatchInsert<T>(IMongoDatabaseFactory mongoDatabaseFactory, TweakSettings tweakSettings, ILogger logger)
{
    public async Task ProcessRecords(IEnumerable<T> records, string collectionName, CancellationToken cancellationToken)
    {
        string tempCollectionName = $"{collectionName}_temp";

        try
        {
            logger.Information("Starting ProcessRecords for collection {CollectionName}", collectionName);

            var collection = mongoDatabaseFactory
                .Database
                .GetCollection<T>(tempCollectionName);

            logger.Information("Got collection reference: {TempCollectionName}", tempCollectionName);

            // Cria índices para T na collection temp
            await MongoIndexHelper.EnsureIndexesForType(collection, cancellationToken);
            logger.Information("Indexes created successfully for {TempCollectionName}", tempCollectionName);

            int batchSize = tweakSettings.RawFilesProcessingSettings.RecordsBatchAmount;
            var buffer = new List<T>(batchSize);

            int chunkNumber = 0;
            int totalInserted = 0;
            bool hasRecords = false;

            logger.Information("Starting to process records in batches of {BatchSize} for {CollectionName}",
                batchSize, collectionName);

            foreach (var record in records)
            {
                hasRecords = true;
                buffer.Add(record);

                if (buffer.Count >= batchSize)
                {
                    chunkNumber++;
                    logger.Information("Processing chunk {ChunkNumber} for {CollectionName} ({RecordCount} records)",
                        chunkNumber, collectionName, buffer.Count);

                    int inserted = await InsertBatchOptimized(collection, buffer, cancellationToken);
                    totalInserted += inserted;

                    logger.Information("Chunk {ChunkNumber} completed. Inserted: {Inserted}, Total so far: {TotalInserted}",
                        chunkNumber, inserted, totalInserted);

                    buffer.Clear();
                }
            }

            // Insere registros restantes
            if (buffer.Count > 0)
            {
                chunkNumber++;
                logger.Information("Processing final chunk {ChunkNumber} for {CollectionName} ({RecordCount} records)",
                    chunkNumber, collectionName, buffer.Count);

                int inserted = await InsertBatchOptimized(collection, buffer, cancellationToken);
                totalInserted += inserted;

                logger.Information("Final chunk completed. Inserted: {Inserted}", inserted);
            }

            if (!hasRecords)
            {
                logger.Warning("No records were processed for {CollectionName}", collectionName);
            }
            else
            {
                logger.Information("ProcessRecords completed successfully for {CollectionName}. Total chunks: {ChunkCount}, Total records inserted: {TotalInserted}",
                    collectionName, chunkNumber, totalInserted);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error processing records for collection {CollectionName} (temp: {TempCollectionName}). Message: {Message}",
                collectionName, tempCollectionName, ex.Message);
            throw;
        }
    }

    private async Task<int> InsertBatchOptimized(IMongoCollection<T> collection, List<T> records, CancellationToken cancellationToken)
    {
        try
        {
            await collection.InsertManyAsync(
                records,
                new InsertManyOptions
                {
                    IsOrdered = false,
                    BypassDocumentValidation = false
                },
                cancellationToken: cancellationToken);

            logger.Debug("Successfully inserted {Count} records into {CollectionName}",
                records.Count, collection.CollectionNamespace.CollectionName);

            return records.Count;
        }
        catch (MongoBulkWriteException ex)
        {
            var duplicateErrors = ex.WriteErrors.Where(e => e.Code == 11000).ToList();
            var otherErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();

            if (duplicateErrors.Any())
            {
                logger.Warning("Duplicate key errors during bulk insert: {Count} duplicates out of {Total} records",
                    duplicateErrors.Count, records.Count);
            }

            if (otherErrors.Any())
            {
                logger.Error(ex, "Critical errors during bulk insert into {CollectionName}: {@Errors}",
                    collection.CollectionNamespace.CollectionName,
                    otherErrors.Select(e => new { e.Code, e.Message, e.Category }));
                throw;
            }

            // Retorna quantos foram inseridos com sucesso
            int successfulInserts = records.Count - ex.WriteErrors.Count;
            logger.Information("Partial insert completed: {Successful} successful, {Failed} failed (duplicates)",
                successfulInserts, duplicateErrors.Count);

            return successfulInserts;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unexpected error during InsertManyAsync into {CollectionName}: {Message}",
                collection.CollectionNamespace.CollectionName, ex.Message);
            throw;
        }
    }
}