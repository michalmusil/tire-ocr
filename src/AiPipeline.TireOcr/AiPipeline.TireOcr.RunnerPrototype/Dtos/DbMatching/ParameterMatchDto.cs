namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record ParameterMatchDto(
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);