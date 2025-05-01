namespace TireOcr.RunnerPrototype.Dtos;

public record EstimatedCostsDto(
    decimal UnitCount,
    string BillingUnit,
    decimal EstimatedCost,
    string EstimatedCostCurrency
);