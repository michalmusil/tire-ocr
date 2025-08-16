using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunBatch;

public record RunBatchAsyncRequest(
    List<IApElement> Inputs,
    List<RunPipelineStepDto> Steps
);