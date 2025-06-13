namespace AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline.Run;

public record RunPipelineDto(
    List<RunPipelineStepDto> Steps
);