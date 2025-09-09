namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record DetectedTireCodeDto(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    string? LoadIndex,
    string? SpeedRating
);