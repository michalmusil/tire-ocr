using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncRequest(
    List<RunPipelineStepDto> Steps
);