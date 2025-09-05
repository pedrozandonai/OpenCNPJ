namespace OpenCnpj.Core.Constants;
public static class Paths
{
    private static readonly string ApplicationName = "OpenCnpj";
    public static readonly string ApplicationTempPath = Path.Combine(Path.GetTempPath(), ApplicationName);
    public static readonly string GovDataFolder = Path.Combine(ApplicationTempPath, "GovData");
}
