using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncResponse(
    PipelineDto Pipeline
);