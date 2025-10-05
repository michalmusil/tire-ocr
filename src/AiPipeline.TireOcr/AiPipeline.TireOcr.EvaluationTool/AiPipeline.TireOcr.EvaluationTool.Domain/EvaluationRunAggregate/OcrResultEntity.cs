using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class OcrResultEntity : TimestampedEntity
{
    public Guid Id { get; }
    public string DetectedCode { get; }
    public string? DetectedManufacturer { get; }
    public decimal? InputUnitCount { get; }
    public decimal? OutputUnitCount { get; }
    public string? BillingUnit { get; }
    public decimal? EstimatedCost { get; }
    public string? EstimatedCostCurrency { get; }
    public long DurationMs { get; }

    public OcrResultEntity(string detectedCode, long durationMs, string? detectedManufacturer = null,
        decimal? inputUnitCount = null, decimal? outputUnitCount = null, string? billingUnit = null,
        decimal? estimatedCost = null, string? estimatedCostCurrency = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        DetectedCode = detectedCode;
        DetectedManufacturer = detectedManufacturer;
        InputUnitCount = inputUnitCount;
        OutputUnitCount = outputUnitCount;
        BillingUnit = billingUnit;
        EstimatedCost = estimatedCost;
        EstimatedCostCurrency = estimatedCostCurrency;
        DurationMs = durationMs;
    }
}