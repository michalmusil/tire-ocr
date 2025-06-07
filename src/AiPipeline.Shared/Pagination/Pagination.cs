namespace TireOcr.Shared.Pagination;

public record Pagination(
    int PageNumber,
    int PageSize,
    int TotalPages,
    int TotalCount,
    bool HasPreviousPage,
    bool HasNextPage
);