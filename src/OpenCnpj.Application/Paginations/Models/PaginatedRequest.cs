using CSharpFunctionalExtensions;

namespace OpenCnpj.Application.Paginations.Models;

public abstract class PaginatedRequest
{
    public virtual int? Page { get; init; } = 1;
    public virtual int? Limit { get; init; } = 25;

    public virtual Result Validate()
    {
        if (Limit.HasValue && Limit >= 1000)
            return Result.Failure("The amount of requested data is too big, the maximum records limit is 1000.");

        return Result.Success();
    }
}
