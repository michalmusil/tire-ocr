namespace TireOcr.RunnerPrototype.Dtos;

public record BatchSummaryDto(
    decimal TotalEstimatedCosts,
    string TotalEstimatedCostsCurrency,
    double TotalDurationMs
);