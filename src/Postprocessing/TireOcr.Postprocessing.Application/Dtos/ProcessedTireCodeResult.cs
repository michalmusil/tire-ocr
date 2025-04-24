namespace TireOcr.Postprocessing.Application.Dtos;

public record ProcessedTireCodeResult(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    int? Width,
    int? AspectRatio,
    string? Construction,
    string? LoadRangeAndIndex,
    string? SpeedRating
);