using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteOcrProcessor;

public record OcrServiceResponseDto(
    [Required] string DetectedCode,
    string? DetectedManufacturer,
    EstimatedCostsResponseDto? EstimatedCosts
)
{
    public OcrResultValueObject ToDomain()
    {
        return new OcrResultValueObject
        {
            DetectedCode = DetectedCode,
            DetectedManufacturer = DetectedManufacturer,
            InputUnitCount = EstimatedCosts?.InputUnitCount,
            OutputUnitCount = EstimatedCosts?.OutputUnitCount,
            BillingUnit = EstimatedCosts?.BillingUnit,
            EstimatedCost = EstimatedCosts?.EstimatedCost,
            EstimatedCostCurrency = EstimatedCosts?.EstimatedCostCurrency,
            DurationMs = 0 // TODO: Add in remote service
        };
    } 
}