namespace TireOcr.Postprocessing.Application.Dtos;

public record ProcessedTireCodeResultDto(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    char? LoadRange,
    int? LoadIndex,
    int? LoadIndex2,
    string? SpeedRating,
    long DurationMs
);