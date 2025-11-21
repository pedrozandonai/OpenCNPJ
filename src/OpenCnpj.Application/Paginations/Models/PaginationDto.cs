namespace OpenCnpj.Application.Paginations.Models;
public class PaginationDto
{
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public int Limit { get; init; }

    public PaginationDto(int currentPage, int limit, int totalItems)
    {
        CurrentPage = currentPage;
        Limit = limit;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling((double)totalItems / limit);
    }
}
