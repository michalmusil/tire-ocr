namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record TireDbEntryDto(
    int Width,
    decimal Diameter,
    int Profile,
    string Construction,
    int? LoadIndex,
    string? SpeedIndex,
    string LoadIndexSpeedIndex
);