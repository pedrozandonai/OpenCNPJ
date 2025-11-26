namespace OpenCnpj.Core.Configurations;

public class TweakSettings
{
    public DownloadSettings DownloadSettings { get; set; } = new();
    public RawFilesProcessingSettings RawFilesProcessingSettings { get; set; } = new();

    public TweakSettings()
    {
        DownloadSettings.Verify();
        RawFilesProcessingSettings.Verify();
    }
}
