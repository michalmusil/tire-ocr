using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.UpdateBatch;

public record UpdateBatchResponse(
    EvaluationRunBatchFullDto Item
);