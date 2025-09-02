namespace OpenCnpj.ConsoleApp.Application.BaseRecords.Abstractions;

public abstract class BaseRecord
{
    public int ID { get; protected set; }
    public string Description { get; private set; }

    protected BaseRecord(int id, string description)
    {
        ID = id;
        Description = description;
    }

    protected BaseRecord()
    {
    }
}
