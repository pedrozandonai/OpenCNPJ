using OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;

namespace OpenCnpj.ConsoleApp.Constants;
public static class Paths
{
    private static readonly string ApplicationName = "OpenCnpj";
    public static readonly string ApplicationTempPath = Path.Combine(Path.GetTempPath(), ApplicationName);
    public static readonly string GovDataFolder = Path.Combine(ApplicationTempPath, "GovData");

    public static string GetRawDirectoryByBatch(Batch batch)
        => Path.Combine(batch.Directory!, "raw");

    public static string GetExtractedDirectoryByBatch(Batch batch)
        => Path.Combine(batch.Directory!, "extracted");
}
