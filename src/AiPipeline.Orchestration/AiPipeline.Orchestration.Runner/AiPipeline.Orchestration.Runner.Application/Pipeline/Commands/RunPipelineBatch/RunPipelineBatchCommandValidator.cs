using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineBatch;

public class RunPipelineBatchCommandValidator : AbstractValidator<RunPipelineBatchCommand>
{
    public RunPipelineBatchCommandValidator()
    {
        RuleFor(c => c.RunPipelineBatchDto.UserId.ToString())
            .IsGuid();

        RuleFor(c => c.RunPipelineBatchDto.Inputs.Count())
            .GreaterThan(1);

        RuleFor(c => c.RunPipelineBatchDto.Steps.Count())
            .GreaterThan(0);

        RuleForEach(x => x.RunPipelineBatchDto.Steps)
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage("Node id of any step cannot be empty.")
            .Must(x => x.ProcedureId.Trim().Length != 0)
            .WithMessage("Procedure id of any step cannot be empty.");
    }
}