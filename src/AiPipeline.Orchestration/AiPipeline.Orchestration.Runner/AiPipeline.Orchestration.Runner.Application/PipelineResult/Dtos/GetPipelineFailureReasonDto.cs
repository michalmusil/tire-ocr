using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public record GetPipelineFailureReasonDto(
    int Code,
    string Reason
)
{
    public static GetPipelineFailureReasonDto FromDomain(PipelineFailureReason failureReason)
    {
        return new GetPipelineFailureReasonDto(failureReason.Code, failureReason.Reason);
    }
}