using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using Fastenshtein;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;

public class LevenshteinTireCodeMatchingService : ITireCodeDbMatchingService
{
    private readonly ITireParamsDbRepository _repository;

    public LevenshteinTireCodeMatchingService(ITireParamsDbRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TireDbMatchDto>> GetOrderedMatchingEntriesForCode(
        DetectedTireCodeDto tireCode, int? limit)
    {
        var existingTireDbEntriesResult = await _repository.GetAllTireParamEntries();
        if (existingTireDbEntriesResult.IsFailure)
            return [];

        var entriesToMatch = existingTireDbEntriesResult.Data!.ToList();
        var stringTireCode = tireCode.PostprocessedTireCode;
        var ratedEntries = entriesToMatch
            .Select(entry =>
            {
                var primaryParameterMatchCount = GetPrimaryParameterMatchCount(tireCode, entry);
                var entryAsString = entry.GetTireCodeString();
                var distance = new Levenshtein(entryAsString)
                    .DistanceFrom(stringTireCode);
                var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, entryAsString, stringTireCode);

                return new TireDbMatchDto(
                    TireEntry: entry,
                    RequiredCharEdits: distance,
                    MatchedMainParameterCount: primaryParameterMatchCount,
                    EstimatedAccuracy: estimatedAccuracy
                );
            });

        var orderedMatches = ratedEntries
            .OrderByDescending(m => m.MatchedMainParameterCount)
            .ThenBy(m => m.RequiredCharEdits)
            .ToList();

        if (limit.HasValue)
            orderedMatches = orderedMatches
                .Take(limit.Value)
                .ToList();

        return orderedMatches;
    }

    private decimal GetAccuracyForLevenshteinDistance(int distance, string string1, string string2)
    {
        var stringLength = Math.Max((decimal)string1.Length, (decimal)string2.Length);
        if (stringLength == 0)
            return 0;
        return 1 - distance / stringLength;
    }

    private int GetPrimaryParameterMatchCount(DetectedTireCodeDto tireCode, ProcessedTireParamsDatabaseEntryDto entry)
    {
        var matchCount = 0;
        if (tireCode.Width == entry.Width)
            matchCount++;

        if (tireCode.AspectRatio.HasValue && tireCode.AspectRatio.Value == entry.Profile)
            matchCount++;

        if (tireCode.Construction is not null &&
            tireCode.Construction.Equals(entry.Construction, StringComparison.OrdinalIgnoreCase))
            matchCount++;

        if (tireCode.Diameter.HasValue && tireCode.Diameter.Value == entry.Diameter)
            matchCount++;


        return matchCount;
    }
}