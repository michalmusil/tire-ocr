namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationMetricsDto(
    decimal ParameterSuccessRate,
    decimal FalsePositiveRate,
    decimal AverageCer,
    decimal AverageInferenceCost,
    decimal AverageLatencyMs,
    decimal? EstimatedAnnualCostUsd,
    decimal? InferenceStability
);