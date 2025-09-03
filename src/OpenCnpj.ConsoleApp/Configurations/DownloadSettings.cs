using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Configurations;
public class DownloadSettings
{
    public int AmountAtTheSameTime { get; set; } = 10;
    public int RetryFailureDownloadsAmount { get; set; } = 5;
    public int ExponencialSecondsIntervalBetweenRetries { get; set; } = 2;
    public bool LogProgress { get; set; } = true;
    public int MbAmountToLog { get; set; } = 50;

    public void Verify()
    {
        if (AmountAtTheSameTime == Settings.SettingMaxValuePossibleFlag)
            AmountAtTheSameTime = Environment.ProcessorCount;

        if (RetryFailureDownloadsAmount == Settings.SettingMaxValuePossibleFlag)
            AmountAtTheSameTime = int.MaxValue;

        if (AmountAtTheSameTime <= 0)
            throw new ArgumentException("The amount of downloads at the same time cannot be lower then ZERO.", nameof(AmountAtTheSameTime));

        if (RetryFailureDownloadsAmount <= 0)
            throw new ArgumentException("The amount of retry failure downloads at the same time cannot be lower then ZERO.", nameof(RetryFailureDownloadsAmount));

        if (ExponencialSecondsIntervalBetweenRetries <= 0)
            throw new ArgumentException("The amount of seconds between each retry cannot be lower then ZERO.", nameof(ExponencialSecondsIntervalBetweenRetries));

        if (MbAmountToLog <= 0)
            throw new ArgumentException("The amount of Mb's to log cannot be lower then ZERO.", nameof(MbAmountToLog));
    }
}
