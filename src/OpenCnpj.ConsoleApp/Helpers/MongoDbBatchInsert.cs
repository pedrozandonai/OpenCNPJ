using MongoDB.Driver;
using OpenCnpj.ConsoleApp.Configurations;
using OpenCnpj.ConsoleApp.Core.Database.Factory.Interfaces;
using Serilog;

namespace OpenCnpj.ConsoleApp.Helpers;
public class MongoDbBatchInsert<T>
{
    private readonly IMongoDatabaseFactory _mongoDatabaseFactory;
    private readonly BatchSettings _batchSettings;
    private readonly ILogger _logger;

    public MongoDbBatchInsert(IMongoDatabaseFactory mongoDatabaseFactory, BatchSettings batchSettings, ILogger logger)
    {
        _mongoDatabaseFactory=mongoDatabaseFactory;
        _batchSettings=batchSettings;
        _logger=logger;
    }

    public async Task ProcessRecords(IEnumerable<T> records, string collectionName, CancellationToken cancellationToken)
    {
        var collection = _mongoDatabaseFactory
            .Database
            .GetCollection<T>(collectionName);

        var buffer = new List<T>(_batchSettings.Size);

        foreach (var record in records)
        {
            buffer.Add(record);

            if (buffer.Count >= _batchSettings.Size)
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
                    IsOrdered = false,  // Continua mesmo se alguns falharem
                    BypassDocumentValidation = false
                },
                cancellationToken: cancellationToken);

            _logger.Debug("Successfully inserted {Count} records", records.Count);
        }
        catch (MongoBulkWriteException ex)
        {
            var otherErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();

            if (otherErrors.Count!=0)
            {
                _logger.Error(ex, "Critical errors during bulk insert: {@Errors}",
                    otherErrors.Select(e => new { e.Code, e.Message }));
                throw; 
            }
        }
    }

    //private async Task InsertBatchOptimized(IMongoCollection<T> collection,
    //    List<T> records, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        // Primeira tentativa: inserção rápida sem verificações
    //        await collection.InsertManyAsync(records,
    //            new InsertManyOptions
    //            {
    //                IsOrdered = false,  // Continua mesmo se alguns falharem
    //                BypassDocumentValidation = false
    //            },
    //            cancellationToken: cancellationToken);

    //        _logger.Debug("Successfully inserted {Count} records", records.Count);
    //    }
    //    catch (MongoBulkWriteException ex)
    //    {
    //        // Separar sucessos de falhas
    //        //var insertedCount = ex.Result.InsertedCount;
    //        var duplicateErrors = ex.WriteErrors.Where(e => e.Code == 11000).ToList();
    //        var otherErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();

    //        if (otherErrors.Any())
    //        {
    //            _logger.Error(ex, "Critical errors during bulk insert: {@Errors}",
    //                otherErrors.Select(e => new { e.Code, e.Message }));
    //            throw;
    //        }

    //        // Log apenas se houver muitas duplicatas (pode indicar problema)
    //        //if (duplicateErrors.Count > records.Count * 0.1) // Mais de 10% duplicatas
    //        //{
    //        //    _logger.Warning("High duplicate rate: {InsertedCount} inserted, {DuplicateCount} duplicates of {TotalCount} total",
    //        //        insertedCount, duplicateErrors.Count, records.Count);
    //        //}
    //        //else
    //        //{
    //        //    _logger.Debug("Batch completed: {InsertedCount} inserted, {DuplicateCount} duplicates",
    //        //        insertedCount, duplicateErrors.Count);
    //        //}
    //    }
    //}
}
