namespace TireOcr.Shared.Pagination;

public record PaginatedCollection<T>
{
    public IReadOnlyCollection<T> Items { get; init; }
    public Pagination Pagination { get; init; }

    public PaginatedCollection()
    {
    }

    public PaginatedCollection(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        Pagination = new Pagination
        (
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: totalPages,
            TotalCount: count,
            HasPreviousPage: pageNumber > 1,
            HasNextPage: pageNumber < totalPages
        );
    }
}