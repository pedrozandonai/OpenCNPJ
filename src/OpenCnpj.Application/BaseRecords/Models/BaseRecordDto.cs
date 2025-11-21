namespace OpenCnpj.Application.BaseRecords.Models;
public abstract record BaseRecordDto
{
    public int ID { get; init; }
    public string Description { get; init; } = string.Empty;

    protected BaseRecordDto()
    {
    }

    protected BaseRecordDto(int id, string description)
    {
        ID = id;
        Description = description;
    }
}
