using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.DeleteEvaluationRun;

public class DeleteEvaluationRunCommandValidator : AbstractValidator<DeleteEvaluationRunCommand>
{
    public DeleteEvaluationRunCommandValidator()
    {
        RuleFor(c => c.RunId.ToString())
            .IsGuid();
    }
}