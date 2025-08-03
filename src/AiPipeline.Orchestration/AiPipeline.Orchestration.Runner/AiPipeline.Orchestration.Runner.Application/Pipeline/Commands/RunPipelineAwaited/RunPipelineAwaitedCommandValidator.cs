using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;

public class RunPipelineAwaitedCommandValidator : AbstractValidator<RunPipelineAwaitedCommand>
{
    public RunPipelineAwaitedCommandValidator()
    {
        RuleFor(x => x.Dto.Steps)
            .NotEmpty();

        RuleForEach(x => x.Dto.Steps)
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage("Node id of any step cannot be empty.")
            .Must(x => x.ProcedureId.Trim().Length != 0)
            .WithMessage("Procedure id of any step cannot be empty.");

        RuleFor(x => x.Timeout.TotalSeconds)
            .GreaterThan(0);
    }
}