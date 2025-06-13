using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncResponse(
    PipelineDto Pipeline
);