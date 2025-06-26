using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;

public class InitPipelineResultCommandValidator : AbstractValidator<InitPipelineResultCommand>
{
    public InitPipelineResultCommandValidator()
    {
        RuleFor(x => x.PipelineId.ToString())
            .IsGuid();
    }
}