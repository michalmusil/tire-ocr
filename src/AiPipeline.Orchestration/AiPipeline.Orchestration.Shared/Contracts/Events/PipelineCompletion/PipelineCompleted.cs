using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;

public record PipelineCompleted(
    Guid PipelineId,
    DateTime CompletedAt,
    IApElement? FinalResult
);