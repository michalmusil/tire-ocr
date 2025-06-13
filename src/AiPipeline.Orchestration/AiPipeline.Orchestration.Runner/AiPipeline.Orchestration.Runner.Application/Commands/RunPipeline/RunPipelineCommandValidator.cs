using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.Commands.RunPipeline;

public class RunPipelineCommandValidator : AbstractValidator<RunPipelineCommand>
{
    public RunPipelineCommandValidator()
    {
        RuleFor(x => x.Dto.Steps)
            .NotEmpty();
        
        RuleForEach(x => x.Dto.Steps)
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage("Node id of any step cannot be empty.")
            .Must(x => x.ProcedureId.Trim().Length != 0)
            .WithMessage("Procedure id of any step cannot be empty.");
    }
}