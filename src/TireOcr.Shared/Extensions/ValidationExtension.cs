using FluentValidation;
using TireOcr.Shared.Pagination;

namespace TireOcr.Shared.Extensions;

public static class ValidationExtension
{
    public static IRuleBuilderOptions<T, PaginationParams> IsValidPagination<T>(
        this IRuleBuilder<T, PaginationParams> ruleBuilder)
    {
        return ruleBuilder
            .Must(p => p.PageNumber > 0 && p.PageSize > 0)
            .WithMessage("Pagination must have positive page number and page size")
            .Must(p => p.PageSize <= 100)
            .WithMessage("Page size can't exceed 100");
    }
    
    public static IRuleBuilderOptions<T, string> IsGuid<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(s => Guid.TryParse(s, out var x))
            .WithMessage((_, s) => $"{s} is not a valid GUID.");
    }
}