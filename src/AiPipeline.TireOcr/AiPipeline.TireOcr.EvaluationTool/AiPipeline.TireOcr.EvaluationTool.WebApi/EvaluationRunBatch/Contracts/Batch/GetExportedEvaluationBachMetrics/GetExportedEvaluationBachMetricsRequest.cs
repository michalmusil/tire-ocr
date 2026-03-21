namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetExportedEvaluationBachMetrics;

public record GetExportedEvaluationBachMetricsRequest(
    IEnumerable<Guid>? InferenceStabilityRelativeBatchIds,
    decimal? FixedExpenditure,
    bool? CalculateVariableExpenditure,
    bool? AverageMetricsWithOtherBatch
);