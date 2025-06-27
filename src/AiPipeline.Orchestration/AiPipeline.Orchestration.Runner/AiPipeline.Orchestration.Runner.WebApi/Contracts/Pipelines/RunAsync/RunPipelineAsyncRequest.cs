using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;

public record RunPipelineAsyncRequest(
    IApElement Input,
    List<RunPipelineStepDto> Steps
);