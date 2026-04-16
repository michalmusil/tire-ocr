using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.UpdateEvaluationBatch;

public class UpdateEvaluationBatchCommandValidator : AbstractValidator<UpdateEvaluationBatchCommand>
{
    public UpdateEvaluationBatchCommandValidator()
    {
        RuleFor(c => c.BatchId.ToString())
            .IsGuid();

        RuleFor(c => c.BatchTitle)
            .Must(bt => bt is null || bt.Trim().Length > 2)
            .WithMessage("Custom batch title must be at least 3 characters long");
    }
}