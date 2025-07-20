using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Events.PipelineFailure;

public record PipelineFailed(
    Guid PipelineId,
    ProcedureIdentifier ProcedureIdentifier,
    DateTime FailedAt,
    int FailureCode,
    string FailureReason,
    string? ExceptionMessage
);