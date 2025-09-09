namespace TireOcr.RunnerPrototype.Dtos.Ocr;

public record EstimatedCostsDto(
    decimal InputUnitCount,
    decimal OutputUnitCount,
    string BillingUnit,
    decimal EstimatedCost,
    string EstimatedCostCurrency
);