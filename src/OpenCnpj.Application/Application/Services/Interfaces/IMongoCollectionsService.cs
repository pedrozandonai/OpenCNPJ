using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Domain;

namespace OpenCnpj.Application.Application.Services.Interfaces;
public interface IMongoCollectionsService
{
    Task<Result> RenameTemporaryCollections(Batch batch, CancellationToken cancellationToken);
}