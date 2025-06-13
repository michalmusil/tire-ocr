using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline.Run;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncRequest(
    List<RunPipelineStepDto> Steps
);