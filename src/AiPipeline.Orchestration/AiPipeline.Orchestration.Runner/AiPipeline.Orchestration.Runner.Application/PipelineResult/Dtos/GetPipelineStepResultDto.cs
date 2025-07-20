using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public record GetPipelineStepResultDto(
    Guid Id,
    string NodeId,
    string NodeProcedureId,
    DateTime FinishedAt,
    bool WasSuccessful,
    IApElement? Result,
    GetPipelineFailureReasonDto? FailureReason
)
{
    public static GetPipelineStepResultDto FromDomain(PipelineStepResult domain)
    {
        return new GetPipelineStepResultDto(
            Id: domain.Id,
            NodeId: domain.NodeId,
            NodeProcedureId: domain.NodeProcedureId,
            FinishedAt: domain.FinishedAt,
            WasSuccessful: domain.WasSuccessful,
            Result: domain.Result,
            FailureReason: domain.FailureReason == null
                ? null
                : GetPipelineFailureReasonDto.FromDomain(domain.FailureReason)
        );
    }
}