using FluentValidation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.RunEvaluationBatch;

public class RunEvaluationBatchCommandValidator : AbstractValidator<RunEvaluationBatchCommand>
{
    public RunEvaluationBatchCommandValidator()
    {
        RuleFor(c => c.ImageUrlsWithExpectedTireCodeLabels)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.ProcessingBatchSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(30);

        RuleFor(c => c.RunConfig)
            .NotNull();

        RuleFor(c => c.BatchTitle)
            .Must(bt => bt is null || bt.Trim().Length > 2)
            .WithMessage("Custom batch title must be at least 3 characters long");
    }
}