using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.MarkPipelineResultBatchCompleted;

public class MarkPipelineResultBatchCompletedCommandValidator: AbstractValidator<MarkPipelineResultBatchCompletedCommand>
{
    public MarkPipelineResultBatchCompletedCommandValidator()
    {
        RuleFor(c => c.BatchResultId.ToString())
            .IsGuid();
    }
}