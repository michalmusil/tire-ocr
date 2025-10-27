using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.RunBatchJsonOnly;

public record RunBatchJsonOnlyResponse(
    EvaluationRunBatchFullDto Result
);