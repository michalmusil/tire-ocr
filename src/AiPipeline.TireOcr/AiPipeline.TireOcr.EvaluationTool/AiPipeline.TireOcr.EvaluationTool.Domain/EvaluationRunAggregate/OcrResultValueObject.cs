using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class OcrResultValueObject : ValueObject
{
    public required string DetectedCode { get; init; }
    public string? DetectedManufacturer { get; init; }
    public decimal? InputUnitCount { get; init; }
    public decimal? OutputUnitCount { get; init; }
    public string? BillingUnit { get; init; }
    public decimal? EstimatedCost { get; init; }
    public string? EstimatedCostCurrency { get; init; }
    public required long DurationMs { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() =>
    [
        DetectedCode, DetectedManufacturer, InputUnitCount, OutputUnitCount, BillingUnit, EstimatedCost,
        EstimatedCostCurrency, DurationMs
    ];
}