using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Queries.GetResultBatchById;

public class GetResultBatchByIdQueryValidator : AbstractValidator<GetResultBatchByIdQuery>
{
    public GetResultBatchByIdQueryValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();

        RuleFor(x => x.UserId.ToString())
            .IsGuid();
    }
}