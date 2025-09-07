namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record TireDbMatchDto(
    ProcessedTireParamsDatabaseEntryDto TireEntry,
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);