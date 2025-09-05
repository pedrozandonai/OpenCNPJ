using CSharpFunctionalExtensions;

namespace OpenCnpj.Core.Configurations;
public class GovSetttings
{
    public string BaseUrl { get; set; } = string.Empty;

    public Result Verify()
    {
        if (string.IsNullOrEmpty(BaseUrl))
            return Result.Failure("The gov base URI is null or empty.");

        return Result.Success();
    }
}
