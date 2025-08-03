using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAwaitedAsync;

public record RunPipelineAwaitedAsyncRequest(
    IApElement Input,
    List<RunPipelineStepDto> Steps,
    long TimeoutSeconds
);