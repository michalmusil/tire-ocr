namespace AiPipeline.Orchestration.Shared.Contracts.Events.PipelineFailure;

public record PipelineFailed(
    Guid PipelineId,
    string ProcedureId,
    DateTime FailedAt,
    int FailureCode,
    string FailureReason,
    string? ExceptionMessage
);