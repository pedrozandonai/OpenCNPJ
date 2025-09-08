using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Simples.Services;
public interface ISimpleService
{
    Task<Result> CreateSimples(CancellationToken cancellationToken);
}