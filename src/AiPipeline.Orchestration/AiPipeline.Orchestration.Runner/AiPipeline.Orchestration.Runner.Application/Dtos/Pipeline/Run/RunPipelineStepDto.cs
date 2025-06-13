namespace AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline.Run;

public record RunPipelineStepDto(
    string NodeId,
    string ProcedureId
);