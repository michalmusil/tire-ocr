namespace TireOcr.RunnerPrototype.Dtos;

public record EstimatedCostsDto(
    decimal InputUnitCount,
    decimal OutputUnitCount,
    string BillingUnit,
    decimal EstimatedCost,
    string EstimatedCostCurrency
);