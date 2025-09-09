using Microsoft.EntityFrameworkCore;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.FileService.Infrastructure.Common.Extensions;

public static class QueryableExtension
{
    public static async Task<PaginatedCollection<T>> ToPaginatedList<T>(this IQueryable<T> queryable,
        PaginationParams pagination)
    {
        var count = await queryable.CountAsync();
        var items = await queryable
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedCollection<T>(items, count, pagination.PageNumber, pagination.PageSize);
    }
}