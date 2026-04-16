namespace AiPipeline.TireOcr.DbMatcher.Application.Dtos;

public record DbMatchingResultDto(
    List<TireDbMatchDto> TireDbMatches,
    string? ManufacturerDbMatch,
    long DurationMs
);