namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.Ocr;

public record EstimatedCostsDto(
    decimal? InputUnitCount,
    decimal? OutputUnitCount,
    string? BillingUnit,
    decimal? EstimatedCost,
    string? EstimatedCostCurrency
);