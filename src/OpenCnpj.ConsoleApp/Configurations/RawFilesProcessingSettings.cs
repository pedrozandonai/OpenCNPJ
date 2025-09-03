using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Configurations;
public class RawFilesProcessingSettings
{
    public int FilesAtTheSameTimeAmount { get; set; } = 10;
    public int RecordsBatchAmount { get; set; } = 1000;

    public void Verify()
    {
        if (FilesAtTheSameTimeAmount == Settings.SettingMaxValuePossibleFlag)
            FilesAtTheSameTimeAmount = Environment.ProcessorCount;

        if (FilesAtTheSameTimeAmount <= 0)
            throw new ArgumentException("The amount of files to process at the same time cannot be lower then ZERO.", nameof(FilesAtTheSameTimeAmount));

        if (RecordsBatchAmount == Settings.SettingMaxValuePossibleFlag)
            RecordsBatchAmount = int.MaxValue;

        if (RecordsBatchAmount <= 0)
            throw new ArgumentException("The amount of records to process at the same time cannot be lower then ZERO.", nameof(RecordsBatchAmount));
    }
}
