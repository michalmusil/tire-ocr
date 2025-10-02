namespace TireOcr.Postprocessing.Application.Dtos;

public record ProcessedTireCodeResultDto(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    string? LoadIndex,
    string? SpeedRating,
    long DurationMs
);