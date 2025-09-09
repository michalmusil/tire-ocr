using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineBatchValidator : AbstractValidator<PipelineBatch>
{
    public PipelineBatchValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();

        RuleFor(x => x.UserId.ToString())
            .IsGuid();

        RuleFor(x => x.Pipelines.Count)
            .GreaterThan(1);
    }
}