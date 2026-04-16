using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.DeleteEvaluationBatch;

public class DeleteEvaluationBatchCommandValidator : AbstractValidator<DeleteEvaluationBatchCommand>
{
    public DeleteEvaluationBatchCommandValidator()
    {
        RuleFor(c => c.BatchId.ToString())
            .IsGuid();
    }
}