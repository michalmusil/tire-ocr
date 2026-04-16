using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Dtos.RemoteOcrProcessor;

public record OcrServiceResponseDto(
    [Required] string DetectedCode,
    string? DetectedManufacturer,
    EstimatedCostsResponseDto? EstimatedCosts,
    [Required] long DurationMs
)
{
    public OcrResultEntity ToDomain()
    {
        return new OcrResultEntity(
            evaluationRunId: Guid.Empty,
            detectedCode: DetectedCode,
            detectedManufacturer: DetectedManufacturer,
            inputUnitCount: EstimatedCosts?.InputUnitCount,
            outputUnitCount: EstimatedCosts?.OutputUnitCount,
            billingUnit: EstimatedCosts?.BillingUnit,
            estimatedCost: EstimatedCosts?.EstimatedCost,
            estimatedCostCurrency: EstimatedCosts?.EstimatedCostCurrency,
            durationMs: DurationMs
        );
    }
}