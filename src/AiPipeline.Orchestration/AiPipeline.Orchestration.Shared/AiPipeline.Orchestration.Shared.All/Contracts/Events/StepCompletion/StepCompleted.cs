using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Events.StepCompletion;

public record StepCompleted(
    Guid PipelineId,
    ProcedureIdentifier ProcedureIdentifier,
    DateTime CompletedAt,
    IApElement? Result
);