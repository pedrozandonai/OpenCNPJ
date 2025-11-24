using CSharpFunctionalExtensions;
using MongoDB.Driver;
using OpenCnpj.Application.Application.Services.Interfaces;
using OpenCnpj.Application.Batches.Batches.Domain;
using OpenCnpj.Application.Batches.Batches.Services;
using OpenCnpj.Core.Database.Factory.Interfaces;

namespace OpenCnpj.Application.Application.Services;
public class MongoCollectionsService(IBatchService batchService, IMongoDatabaseFactory mongoDatabaseFactory) : IMongoCollectionsService
{
    public async Task<Result> RenameTemporaryCollections(Batch batch, CancellationToken cancellationToken)
    {
        const string temporaryCollectionTag = "_temp";

        var startProcessingCsvFilesResult = await batchService.UpdateBatch(batch, batch.StartRenamingMongoCollections, cancellationToken);
        if (startProcessingCsvFilesResult.IsFailure)
            return startProcessingCsvFilesResult;

        var collections = await mongoDatabaseFactory.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken);
        var temporaryCollectionNames = await collections.ToListAsync(cancellationToken);

        foreach (var temporaryCollectionName in temporaryCollectionNames.Where(temporaryCollectionName => temporaryCollectionName.Contains(temporaryCollectionTag)))
        {
            string newCollectionName = temporaryCollectionName.Replace(temporaryCollectionTag, "");
            if (temporaryCollectionNames.Contains(newCollectionName))
                await mongoDatabaseFactory.Database.DropCollectionAsync(newCollectionName, cancellationToken: cancellationToken); // Se a coleção já existe os dados do gov foram atualizados e dá pra dropar toda coleção pra substituir pelos dados atualizados.

            await mongoDatabaseFactory.Database.RenameCollectionAsync(temporaryCollectionName, newCollectionName, cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
}
