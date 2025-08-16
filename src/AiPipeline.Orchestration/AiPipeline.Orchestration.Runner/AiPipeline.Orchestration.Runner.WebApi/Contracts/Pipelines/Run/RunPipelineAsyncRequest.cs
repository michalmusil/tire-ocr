using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.Run;

public record RunPipelineAsyncRequest(
    IApElement Input,
    List<RunPipelineStepDto> Steps
);