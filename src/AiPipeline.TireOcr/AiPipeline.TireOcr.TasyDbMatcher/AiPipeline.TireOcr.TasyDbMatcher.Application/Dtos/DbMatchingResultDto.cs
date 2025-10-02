namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record DbMatchingResultDto(
    List<TireDbMatchDto> TireDbMatches,
    string? ManufacturerDbMatch,
    long DurationMs
);