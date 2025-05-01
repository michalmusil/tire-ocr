namespace TireOcr.RunnerPrototype.Dtos;

public record BillingDto(
    decimal InputAmount,
    decimal OutputAmount,
    string Unit
);