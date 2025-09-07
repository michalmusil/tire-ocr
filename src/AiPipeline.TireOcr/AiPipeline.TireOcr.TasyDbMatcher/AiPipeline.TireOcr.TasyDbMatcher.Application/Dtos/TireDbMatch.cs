namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record TireDbMatch(
    ProcessedTireParamsDatabaseEntryDto TireEntry,
    int RequiredCharEdits,
    double EstimatedAccuracy
);