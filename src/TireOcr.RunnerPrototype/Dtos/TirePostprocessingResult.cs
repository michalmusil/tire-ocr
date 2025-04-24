namespace TireOcr.RunnerPrototype.Dtos;

public record TirePostprocessingResult(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    int? Width,
    int? AspectRatio,
    string? Construction,
    int? Diameter,
    string? LoadRangeAndIndex,
    string? SpeedRating
);