namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationStatisticsDto(
    decimal ParameterSuccessRate,
    decimal FalsePositiveRate,
    decimal AverageCer,
    decimal AverageInferenceCost,
    decimal AverageLatencyMs
);