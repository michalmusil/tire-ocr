using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.UpdateEvaluationRun;

public class UpdateEvaluationRunCommandValidator : AbstractValidator<UpdateEvaluationRunCommand>
{
    public UpdateEvaluationRunCommandValidator()
    {
        RuleFor(c => c.RunId.ToString())
            .IsGuid();

        RuleFor(c => c.RunTitle)
            .Must(rt => rt is null || rt.Trim().Length > 2)
            .WithMessage("Custom run title must be at least 3 characters long");
    }
}