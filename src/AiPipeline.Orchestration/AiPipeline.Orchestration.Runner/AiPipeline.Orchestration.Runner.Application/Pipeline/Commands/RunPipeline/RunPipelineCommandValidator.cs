using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;

public class RunPipelineCommandValidator : AbstractValidator<RunPipelineCommand>
{
    public RunPipelineCommandValidator()
    {
        RuleFor(x => x.Dto.Steps)
            .NotEmpty();

        RuleFor(x => x.Dto.UserId.ToString())
            .IsGuid();
        
        RuleFor(x => x.Dto.Steps.Count)
            .GreaterThan(0);

        RuleForEach(x => x.Dto.Steps)
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage("Node id of any step cannot be empty.")
            .Must(x => x.ProcedureId.Trim().Length != 0)
            .WithMessage("Procedure id of any step cannot be empty.");
    }
}