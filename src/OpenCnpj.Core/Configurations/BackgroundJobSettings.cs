namespace OpenCnpj.Core.Configurations;

public class BackgroundJobSettings
{
    public string Cron { get; set; } = string.Empty;
    public int RetryAmount { get; set; } = 3;
    public int RetryDelay { get; set; } = 30;
    public bool IsActive { get; set; } = false;
}
