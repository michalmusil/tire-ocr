using AiPipeline.TireOcr.DbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.DbMatcher.WebApi.Contracts.GetMatchesInTireDb;

public record GetMatchesInTireDbRequest(DetectedTireCodeDto TireCode, string? Manufacturer);