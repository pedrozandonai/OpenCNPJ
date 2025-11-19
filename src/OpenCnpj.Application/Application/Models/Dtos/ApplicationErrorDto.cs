namespace OpenCnpj.Application.Application.Models.Dtos;

public record ApplicationErrorDto
{
    public string Error { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public Guid TraceId { get; set; }
    public string Description { get; set; }

    public ApplicationErrorDto(string error, string title, int status, Guid traceId, string description)
    {
        Error = error;
        Title = title;
        Status = status;
        TraceId = traceId;
        Description = description;
    }
}
