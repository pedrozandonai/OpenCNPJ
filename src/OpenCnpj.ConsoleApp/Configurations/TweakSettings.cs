namespace OpenCnpj.ConsoleApp.Configurations;

public class TweakSettings
{
    public DownloadSettings DownloadSettings { get; set; } = new();
    public RawFilesProcessingSettings RawFilesProcessingSettings { get; set; } = new();
    public FormatRawDataSettings FormatRawDataSettings { get; set; } = new();

    public TweakSettings()
    {
        DownloadSettings.Verify();
        RawFilesProcessingSettings.Verify();
        FormatRawDataSettings.Verify();
    }
}
