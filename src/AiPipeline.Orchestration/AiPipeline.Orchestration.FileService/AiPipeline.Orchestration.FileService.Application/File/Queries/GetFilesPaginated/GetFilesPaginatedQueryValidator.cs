using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryValidator : AbstractValidator<GetFilesPaginatedQuery>
{
    public GetFilesPaginatedQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();

        RuleFor(q => q.UserId.ToString())
            .IsGuid();
    }
}