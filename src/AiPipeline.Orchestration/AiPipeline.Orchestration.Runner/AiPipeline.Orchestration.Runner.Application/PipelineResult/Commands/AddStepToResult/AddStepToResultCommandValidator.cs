using FluentValidation;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.AddStepToResult;

public class AddStepToResultCommandValidator : AbstractValidator<AddStepToResultCommand>
{
    public AddStepToResultCommandValidator()
    {
        RuleFor(x => x.PipelineId.ToString())
            .IsGuid();
        
        RuleFor(x => x.Dto.Id.ToString())
            .IsGuid();
        RuleFor(x => x.Dto.ResultId.ToString())
            .IsGuid();
        RuleFor(x => x.Dto.NodeId)
            .NotEmpty();
        RuleFor(x => x.Dto.NodeProcedureId)
            .NotEmpty();
        RuleFor(x => x.Dto)
            .Must(x => x.WasSuccessful || x.FailureReason != null)
            .WithMessage("Failed pipeline steps must contain a failure reason");
    }
}