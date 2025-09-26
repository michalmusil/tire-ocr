namespace TireOcr.RunnerPrototype.Dtos.DbMatching;

public record DbMatcherServiceResponseDto(
    List<TireDbMatchDto> OrderedTireCodeDbMatches,
    string? ManufacturerDbMatch
);