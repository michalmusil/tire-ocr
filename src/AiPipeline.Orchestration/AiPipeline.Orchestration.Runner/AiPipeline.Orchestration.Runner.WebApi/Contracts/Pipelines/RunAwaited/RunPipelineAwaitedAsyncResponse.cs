using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAwaited;

public record RunPipelineAwaitedAsyncResponse(GetPipelineResultDto Result);