using FluentValidation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;

public class RunEvaluationBatchCommandValidator : AbstractValidator<RunEvaluationBatchCommand>
{
    public RunEvaluationBatchCommandValidator()
    {
        RuleFor(c => c.InputImagesWithExpectedTireCodes)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.ProcessingBatchSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(30);

        RuleFor(c => c.RunConfig)
            .NotNull();
    }
}