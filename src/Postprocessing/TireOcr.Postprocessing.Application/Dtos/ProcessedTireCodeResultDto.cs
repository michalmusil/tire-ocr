namespace TireOcr.Postprocessing.Application.Dtos;

public record ProcessedTireCodeResultDto(
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