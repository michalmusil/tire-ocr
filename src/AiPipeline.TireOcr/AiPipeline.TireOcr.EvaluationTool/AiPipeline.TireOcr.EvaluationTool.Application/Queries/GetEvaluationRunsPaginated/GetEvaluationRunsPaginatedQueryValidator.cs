using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunsPaginated;

public class GetEvaluationRunsPaginatedQueryValidator : AbstractValidator<GetEvaluationRunsPaginatedQuery>
{
    public GetEvaluationRunsPaginatedQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}