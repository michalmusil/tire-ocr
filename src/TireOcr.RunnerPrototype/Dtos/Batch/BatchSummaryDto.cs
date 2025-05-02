namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record BatchSummaryDto(
    decimal TotalEstimatedCosts,
    string TotalEstimatedCostsCurrency,
    double TotalDurationMs,
    PipelineCompletionSuccessRateDto PipelineCompletionSuccessRate
);