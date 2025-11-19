using CSharpFunctionalExtensions;
using OpenCnpj.Application.Batches.Domain;

namespace OpenCnpj.Application.Application.Services.Interfaces;
public interface IOpenCnpjScrapperService
{
    Task<Result> ExecuteAsync(Batch batch, CancellationToken cancellationToken);
}