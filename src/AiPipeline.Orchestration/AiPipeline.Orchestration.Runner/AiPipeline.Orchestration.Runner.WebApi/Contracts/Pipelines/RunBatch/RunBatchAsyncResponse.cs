using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunBatch;

public record RunBatchAsyncResponse(
    GetPipelineBatchDto Batch
);