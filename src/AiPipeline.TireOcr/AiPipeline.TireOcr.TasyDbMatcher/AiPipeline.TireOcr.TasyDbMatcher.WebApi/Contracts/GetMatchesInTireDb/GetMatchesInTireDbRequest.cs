using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.TasyDbMatcher.WebApi.Contracts.GetMatchesInTireDb;

public record GetMatchesInTireDbRequest(DetectedTireCodeDto TireCode);