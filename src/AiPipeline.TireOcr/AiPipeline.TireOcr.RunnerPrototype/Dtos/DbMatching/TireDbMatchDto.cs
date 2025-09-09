namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record TireDbMatchDto(
    TireDbEntryDto TireEntry,
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);