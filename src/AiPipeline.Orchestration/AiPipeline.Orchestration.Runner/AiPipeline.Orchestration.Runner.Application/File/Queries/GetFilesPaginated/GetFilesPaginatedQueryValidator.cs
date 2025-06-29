using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryValidator : AbstractValidator<GetFilesPaginatedQuery>
{
    public GetFilesPaginatedQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}