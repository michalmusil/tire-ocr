using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineBatches.GetBatchResultsById;

public record GetBatchResultsByIdResponse(
    GetPipelineResultBatchDto Batch
);