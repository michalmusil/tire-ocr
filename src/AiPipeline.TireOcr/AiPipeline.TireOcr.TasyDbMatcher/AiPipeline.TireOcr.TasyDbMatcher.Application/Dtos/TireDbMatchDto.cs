namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record TireDbMatchDto(
    ProcessedTireParamsDatabaseEntryDto TireEntry,
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