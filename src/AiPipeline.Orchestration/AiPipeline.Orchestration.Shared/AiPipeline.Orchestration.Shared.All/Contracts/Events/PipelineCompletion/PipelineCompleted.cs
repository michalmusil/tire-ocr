using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Events.PipelineCompletion;

public record PipelineCompleted(
    Guid PipelineId,
    DateTime CompletedAt,
    IApElement? FinalResult
);