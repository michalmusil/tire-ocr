namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record TireDbMatchDto(
    TireDbEntryDto TireEntry,
    int TotalRequiredCharEdits,
    int MatchedMainParameterCount,
    decimal EstimatedAccuracy,
    ParameterMatchDto WidthMatch,
    ParameterMatchDto DiameterMatch,
    ParameterMatchDto ProfileMatch,
    ParameterMatchDto? ConstructionMatch,
    ParameterMatchDto LoadIndexMatch,
    ParameterMatchDto? LoadIndex2Match,
    ParameterMatchDto SpeedIndexMatch
);