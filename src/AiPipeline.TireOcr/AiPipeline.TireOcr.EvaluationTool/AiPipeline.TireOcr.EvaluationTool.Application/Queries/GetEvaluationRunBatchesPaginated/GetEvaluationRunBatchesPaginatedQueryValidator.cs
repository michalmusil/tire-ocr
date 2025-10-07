using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchesPaginated;

public class GetEvaluationRunBatchesPaginatedQueryValidator : AbstractValidator<GetEvaluationRunBatchesPaginatedQuery>
{
    public GetEvaluationRunBatchesPaginatedQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}