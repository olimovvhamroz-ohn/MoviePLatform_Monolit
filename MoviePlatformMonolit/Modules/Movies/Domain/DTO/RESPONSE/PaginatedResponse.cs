namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => PageSize > 0
        ? (TotalCount + PageSize - 1) / PageSize
        : 0;

    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}