using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.Ocr;

public record OcrResultDto(
    string DetectedCode,
    string? DetectedManufacturer,
    EstimatedCostsDto? EstimatedCosts,
    long DurationMs
)
{
    public static OcrResultDto FromDomain(OcrResultEntity domain)
    {
        var shouldIncludeEstimatedCosts = domain.InputUnitCount != null || domain.OutputUnitCount != null ||
                                          domain.BillingUnit != null || domain.EstimatedCost != null ||
                                          domain.EstimatedCostCurrency != null;
        return new OcrResultDto(
            DetectedCode: domain.DetectedCode,
            DetectedManufacturer: domain.DetectedManufacturer,
            DurationMs: domain.DurationMs,
            EstimatedCosts: shouldIncludeEstimatedCosts
                ? new EstimatedCostsDto(
                    InputUnitCount: domain.InputUnitCount,
                    OutputUnitCount: domain.OutputUnitCount,
                    BillingUnit: domain.BillingUnit,
                    EstimatedCostCurrency: domain.EstimatedCostCurrency,
                    EstimatedCost: domain.EstimatedCost)
                : null
        );
    }
}