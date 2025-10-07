using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunById;

public class GetEvaluationRunBatchByIdQueryValidator : AbstractValidator<GetEvaluationRunByIdQuery>
{
    public GetEvaluationRunBatchByIdQueryValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();
    }
}