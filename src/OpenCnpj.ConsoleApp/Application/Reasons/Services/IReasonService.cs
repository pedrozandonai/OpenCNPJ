using CSharpFunctionalExtensions;

namespace OpenCnpj.ConsoleApp.Application.Reasons.Services;
public interface IReasonService
{
    Task<Result> CreateReasons(CancellationToken cancellationToken);
}