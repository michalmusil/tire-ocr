namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Dtos.RemoteOcrProcessor;

public record EstimatedCostsResponseDto(
    decimal InputUnitCount,
    decimal OutputUnitCount,
    string BillingUnit,
    decimal EstimatedCost,
    string EstimatedCostCurrency
);