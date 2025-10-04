namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record TireDbEntryDto(
    decimal Width,
    decimal Diameter,
    decimal Profile,
    string Construction,
    int? LoadIndex,
    int? LoadIndex2,
    string? SpeedIndex,
    string LoadIndexSpeedIndex
);