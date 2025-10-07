using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.RunBatchJsonOnly;

public record RunBatchJsonOnlyResponse(
    EvaluationRunBatchFullDto Result
);