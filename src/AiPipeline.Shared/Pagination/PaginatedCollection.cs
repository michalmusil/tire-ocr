namespace TireOcr.Shared.Pagination;

public record PaginatedCollection<T>
{
    public IReadOnlyCollection<T> Items { get; init; }
    public Pagination Pagination { get; init; }

    public PaginatedCollection()
    {
    }

    public PaginatedCollection(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        Pagination = new Pagination
        (
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: totalPages,
            TotalCount: totalCount,
            HasPreviousPage: pageNumber > 1,
            HasNextPage: pageNumber < totalPages
        );
    }

    public static PaginatedCollection<T> Empty<T>(int pageNumber, int pageSize)
    {
        return new PaginatedCollection<T>(
            items: new List<T>(),
            totalCount: 0,
            pageNumber: pageNumber,
            pageSize: pageSize
        );
    }
}