using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetResultOfPipeline;

public class GetResultOfPipelineQueryValidator : AbstractValidator<GetResultOfPipelineQuery>
{
    public GetResultOfPipelineQueryValidator()
    {
        RuleFor(q => q.PipelineId.ToString())
            .IsGuid();
    }
}