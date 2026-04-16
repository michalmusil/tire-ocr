using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.RunBatchForm;

public record RunBatchFormResponse(
    EvaluationRunBatchFullDto Result
);