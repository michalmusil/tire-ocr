using AiPipeline.TireOcr.DbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.DbMatcher.WebApi.Contracts.GetMatchesInTireDb;

public record GetMatchesInTireDbResponse(
    List<TireDbMatchDto> OrderedTireCodeDbMatches,
    string? ManufacturerDbMatch,
    long DurationMs
);