using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Configurations;
public class FormatRawDataSettings
{
    public int RecordsBatchAmount { get; set; } = -1;
    public int AmountAtTheSameTime { get; set; } = 50;

    public void Verify()
    {
        if (RecordsBatchAmount == Settings.SettingMaxValuePossibleFlag)
            RecordsBatchAmount = int.MaxValue;

        if (RecordsBatchAmount <= 0)
            throw new ArgumentException("The amount of records to process at the same time cannot be lower then ZERO.", nameof(RecordsBatchAmount));

        if (AmountAtTheSameTime == Settings.SettingMaxValuePossibleFlag)
            AmountAtTheSameTime = Environment.ProcessorCount;

        if (AmountAtTheSameTime <= 0)
            throw new ArgumentException("The amount of threads to process cannot be lower then ZERO.", nameof(AmountAtTheSameTime));
    }
}
