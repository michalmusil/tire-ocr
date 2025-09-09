using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record RunPipelineDto(
    Guid UserId,
    IApElement Input,
    List<RunPipelineStepDto> Steps
);