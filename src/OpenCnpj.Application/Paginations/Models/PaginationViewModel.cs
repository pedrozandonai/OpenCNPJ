namespace OpenCnpj.Application.Paginations.Models;
public class PaginationViewModel<T>
{
    public IEnumerable<T> Data { get; init; }
    public PaginationDto Pagination { get; init; }

    public PaginationViewModel(IEnumerable<T> data, PaginationDto pagination)
    {
        Data = data;
        Pagination = pagination;
    }
}
