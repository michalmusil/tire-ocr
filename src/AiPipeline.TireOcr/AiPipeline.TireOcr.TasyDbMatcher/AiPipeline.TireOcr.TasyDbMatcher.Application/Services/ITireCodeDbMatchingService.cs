using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Services;

public interface ITireCodeDbMatchingService
{
    public Task<List<TireDbMatchDto>> GetOrderedMatchingEntriesForCode(
        DetectedTireCodeDto tireCode,
        IEnumerable<ProcessedTireParamsDatabaseEntryDto> entriesToMatch,
        int? limit
    );
}