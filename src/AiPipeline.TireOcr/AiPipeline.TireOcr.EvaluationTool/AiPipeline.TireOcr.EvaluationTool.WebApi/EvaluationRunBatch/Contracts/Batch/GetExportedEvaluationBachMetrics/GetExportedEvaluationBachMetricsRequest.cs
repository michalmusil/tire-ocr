namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetExportedEvaluationBachMetrics;

public record GetExportedEvaluationBachMetricsRequest(
    Guid? InferenceStabilityRelativeBatchId,
    decimal? AnnualFixedCostUsd,
    int? ExpectedAnnualInferences,
    bool? AverageMetricsWithOtherBatch
);