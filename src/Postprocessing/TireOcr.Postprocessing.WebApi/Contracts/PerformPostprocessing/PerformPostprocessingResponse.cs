namespace TireOcr.Postprocessing.WebApi.Contracts.PerformPostprocessing;

public record PerformPostprocessingResponse(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    int? Width,
    int? AspectRatio,
    string? Construction,
    int? Diameter,
    string? LoadIndex,
    string? SpeedRating
);