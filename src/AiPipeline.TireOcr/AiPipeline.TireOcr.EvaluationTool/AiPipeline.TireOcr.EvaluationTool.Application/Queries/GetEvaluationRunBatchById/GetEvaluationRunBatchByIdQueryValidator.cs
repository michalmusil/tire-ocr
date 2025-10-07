using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchById;

public class GetEvaluationRunBatchByIdQueryValidator : AbstractValidator<GetEvaluationRunBatchByIdQuery>
{
    public GetEvaluationRunBatchByIdQueryValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();
    }
}