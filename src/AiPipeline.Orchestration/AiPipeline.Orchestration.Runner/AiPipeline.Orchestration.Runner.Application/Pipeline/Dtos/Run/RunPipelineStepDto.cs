namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record RunPipelineStepDto(
    string NodeId,
    string ProcedureId,
    string? OutputValueSelector
);