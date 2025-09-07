using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using Fastenshtein;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;

public class LevenshteinTireCodeMatchingService : ITireCodeDbMatchingService
{
    public Task<List<TireDbMatch>> GetOrderedMatchingEntriesForCode(
        DetectedTireCodeDto tireCode, IEnumerable<ProcessedTireParamsDatabaseEntryDto> entriesToMatch, int? limit)
    {
        var entryList = entriesToMatch.ToList();
        var stringTireCode = tireCode.PostprocessedTireCode;
        var ratedEntries = new List<TireDbMatch>();

        var endIndex = limit ?? entryList.Count;
        for (var i = 0; i < endIndex; i++)
        {
            var entry = entryList.ElementAt(i);
            var entryAsString = entry.GetTireCodeString();
            var distance = new Levenshtein(entryAsString)
                .DistanceFrom(stringTireCode);
            var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, entryAsString, stringTireCode);

            var ratedEntry = new TireDbMatch(entry, distance, estimatedAccuracy);
            ratedEntries.Add(ratedEntry);
        }

        var orderedMatches = ratedEntries
            .OrderByDescending(m => m.RequiredCharEdits)
            .ToList();

        return Task.FromResult(orderedMatches);
    }

    private double GetAccuracyForLevenshteinDistance(int distance, string string1, string string2)
    {
        var stringLength = Math.Max((double)string1.Length, (double)string2.Length);
        return 1 - distance / stringLength;
    }
}