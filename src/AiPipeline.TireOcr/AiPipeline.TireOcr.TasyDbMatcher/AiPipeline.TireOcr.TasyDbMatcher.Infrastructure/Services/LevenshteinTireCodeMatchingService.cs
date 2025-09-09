using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using Fastenshtein;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;

public class LevenshteinTireCodeMatchingService : ITireCodeDbMatchingService
{
    public Task<List<TireDbMatchDto>> GetOrderedMatchingEntriesForCode(
        DetectedTireCodeDto tireCode, IEnumerable<ProcessedTireParamsDatabaseEntryDto> entriesToMatch, int? limit)
    {
        var stringTireCode = tireCode.PostprocessedTireCode;
        var ratedEntries = entriesToMatch
            .Select(entry =>
            {
                var entryAsString = entry.GetTireCodeString();
                var distance = new Levenshtein(entryAsString)
                    .DistanceFrom(stringTireCode);
                var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, entryAsString, stringTireCode);

                return new TireDbMatchDto(entry, distance, estimatedAccuracy);
            });

        var orderedMatches = ratedEntries
            .OrderBy(m => m.RequiredCharEdits)
            .ToList();

        if (limit.HasValue)
            orderedMatches = orderedMatches
                .Take(limit.Value)
                .ToList();

        return Task.FromResult(orderedMatches);
    }

    private decimal GetAccuracyForLevenshteinDistance(int distance, string string1, string string2)
    {
        var stringLength = Math.Max((decimal)string1.Length, (decimal)string2.Length);
        if (stringLength == 0)
            return 0;
        return 1 - distance / stringLength;
    }
}