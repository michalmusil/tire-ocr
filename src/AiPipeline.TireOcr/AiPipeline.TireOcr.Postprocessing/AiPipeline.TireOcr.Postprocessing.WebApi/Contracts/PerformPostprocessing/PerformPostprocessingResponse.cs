namespace TireOcr.Postprocessing.WebApi.Contracts.PerformPostprocessing;

public record PerformPostprocessingResponse(
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