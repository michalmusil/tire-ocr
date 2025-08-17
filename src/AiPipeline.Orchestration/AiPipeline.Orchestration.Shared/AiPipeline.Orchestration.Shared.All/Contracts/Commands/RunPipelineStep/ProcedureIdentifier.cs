namespace AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;

public record ProcedureIdentifier(
    string NodeId,
    string ProcedureId,
    int OrderInPipeline
);