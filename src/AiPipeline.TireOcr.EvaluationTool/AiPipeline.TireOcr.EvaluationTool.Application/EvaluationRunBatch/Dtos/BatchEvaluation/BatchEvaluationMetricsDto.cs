namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationMetricsDto(
    decimal ParameterSuccessRate,
    decimal FalsePositiveRate,
    decimal AverageCer,
    decimal AverageVariableInferenceExpenditure,
    decimal MedianLatencyMs,
    decimal? NormalizedInferenceExpenditure,
    decimal? InferenceStability
);