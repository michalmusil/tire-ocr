using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public record GetPipelineStepResultDto(
    int Order,
    Guid Id,
    string NodeId,
    string NodeProcedureId,
    DateTime FinishedAt,
    bool WasSuccessful,
    string? OutputValueSelector,
    IApElement? Result,
    GetPipelineFailureReasonDto? FailureReason
)
{
    public static GetPipelineStepResultDto FromDomain(PipelineStepResult domain)
    {
        return new GetPipelineStepResultDto(
            Order: domain.Order,
            Id: domain.Id,
            NodeId: domain.NodeId,
            NodeProcedureId: domain.NodeProcedureId,
            FinishedAt: domain.FinishedAt,
            WasSuccessful: domain.WasSuccessful,
            OutputValueSelector: domain.OutputValueSelector,
            Result: domain.Result,
            FailureReason: domain.FailureReason == null
                ? null
                : GetPipelineFailureReasonDto.FromDomain(domain.FailureReason)
        );
    }
}