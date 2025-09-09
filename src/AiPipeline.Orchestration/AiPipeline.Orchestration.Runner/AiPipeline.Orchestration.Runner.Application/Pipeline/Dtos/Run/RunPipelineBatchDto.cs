using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

public record RunPipelineBatchDto(
    Guid UserId,
    IEnumerable<IApElement> Inputs,
    List<RunPipelineStepDto> Steps
);