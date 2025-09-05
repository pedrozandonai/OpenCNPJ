using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Reasons.Services;
public interface IReasonService
{
    Task<Result> CreateReasons(CancellationToken cancellationToken);
}