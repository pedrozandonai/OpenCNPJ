using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace OpenCnpj.ConsoleApp.Configurations;
public class GovSetttings
{
    public string BaseUrl { get; set; } = string.Empty;
    public IEnumerable<string> Keywords { get; set; } = [];

    public Result Verify()
    {
        if (string.IsNullOrEmpty(BaseUrl))
            return Result.Failure("The gov base URI is null or empty.");

        if (!Keywords.Any())
            return Result.Failure("The gov keywords are empty.");

        return Result.Success();
    }
}
