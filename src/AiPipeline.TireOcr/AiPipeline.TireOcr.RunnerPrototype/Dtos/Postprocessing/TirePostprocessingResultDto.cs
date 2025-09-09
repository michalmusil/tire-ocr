namespace TireOcr.RunnerPrototype.Dtos.Postprocessing;

public record TirePostprocessingResultDto(
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