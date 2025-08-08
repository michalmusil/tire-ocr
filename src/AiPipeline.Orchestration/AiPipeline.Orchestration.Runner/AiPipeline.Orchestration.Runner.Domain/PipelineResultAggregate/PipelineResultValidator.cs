using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

public class PipelineResultValidator : AbstractValidator<PipelineResult>
{
    public PipelineResultValidator()
    {
        RuleFor(x => x.Id.ToString())
            .NotEmpty();

        RuleFor(x => x.PipelineId.ToString())
            .NotEmpty();

        RuleFor(x => x.UserId.ToString())
            .NotEmpty();

        RuleForEach(x => x.StepResults)
            .Must(x => x.Id.ToString().Trim().Length != 0)
            .WithMessage(x => "Pipeline step result id must not be empty")
            .Must(x => x.ResultId.ToString().Trim().Length != 0)
            .WithMessage(x => "Pipeline step result id must not be empty")
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage(x => "Pipeline node id must not be empty")
            .Must(x => x.NodeProcedureId.Trim().Length != 0)
            .WithMessage(x => "Pipeline node procedure id must not be empty")
            .Must(x => x.WasSuccessful || x.FailureReason is not null)
            .WithMessage(x => "Non-successful pipeline step results must have a failure reason");
    }
}