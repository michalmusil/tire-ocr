namespace AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;

public record ProcedureIdentifier(
    string NodeId,
    string ProcedureId
);