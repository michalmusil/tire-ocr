using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;

public class MarkPipelineCompletedCommandValidator : AbstractValidator<MarkPipelineCompletedCommand>
{
    public MarkPipelineCompletedCommandValidator()
    {
        RuleFor(x => x.PipelineId.ToString())
            .IsGuid();
    }
}