using AiPipeline.TireOcr.DbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.DbMatcher.Application.Services;

public interface ITireCodeDbMatchingService
{
    public Task<List<TireDbMatchDto>> GetOrderedMatchingEntriesForCode(
        DetectedTireCodeDto tireCode,
        int? limit
    );
}