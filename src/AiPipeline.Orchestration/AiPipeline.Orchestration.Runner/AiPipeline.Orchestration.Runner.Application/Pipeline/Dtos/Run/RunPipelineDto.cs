using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record RunPipelineDto(
    IApElement Input,
    List<RunPipelineStepDto> Steps
);