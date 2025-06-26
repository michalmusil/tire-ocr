using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.Contracts.Events.StepCompletion;

public record StepCompleted(
    Guid PipelineId,
    ProcedureIdentifier ProcedureIdentifier,
    DateTime CompletedAt,
    IApElement? Result
);