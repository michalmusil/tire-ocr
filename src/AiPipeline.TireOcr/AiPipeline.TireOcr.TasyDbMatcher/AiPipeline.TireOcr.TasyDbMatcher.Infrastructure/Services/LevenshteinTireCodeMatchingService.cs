using System.Globalization;
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
        var ratedEntries = entriesToMatch
            .Select(entry => GetMatchForCodeAndDbEntry(tireCode, entry));

        var orderedMatches = ratedEntries
            .OrderByDescending(m => m.MatchedMainParameterCount)
            .ThenBy(m => m.TotalRequiredCharEdits)
            .ToList();

        if (limit.HasValue)
            orderedMatches = orderedMatches
                .Take(limit.Value)
                .ToList();

        return orderedMatches;
    }

    private TireDbMatchDto GetMatchForCodeAndDbEntry(
        DetectedTireCodeDto tireCode,
        ProcessedTireParamsDatabaseEntryDto tireEntry
    )
    {
        var parameterMatches = new List<ParameterMatchDto>();
        var widthMatch = GetParameterMatch(tireCode.Width?.ToString() ?? "",
            tireEntry.Width.ToString(CultureInfo.InvariantCulture));
        parameterMatches.Add(widthMatch);

        var diameterMatch = GetParameterMatch(tireCode.Diameter?.ToString() ?? "",
            tireEntry.Diameter.ToString(CultureInfo.InvariantCulture));
        parameterMatches.Add(diameterMatch);

        var profileMatch = GetParameterMatch(tireCode.AspectRatio?.ToString() ?? "",
            tireEntry.Profile.ToString(CultureInfo.InvariantCulture));
        parameterMatches.Add(profileMatch);

        var constructionMatch = tireEntry.Construction is not null || tireCode.Construction is not null
            ? GetParameterMatch(tireCode.Construction ?? "",
                tireEntry.Construction ?? "")
            : null;
        if (constructionMatch is not null)
            parameterMatches.Add(constructionMatch);

        var loadIndexMatch = GetParameterMatch(tireCode.LoadIndex?.ToString() ?? "",
            tireEntry.LoadIndex?.ToString(CultureInfo.InvariantCulture) ?? "");
        parameterMatches.Add(loadIndexMatch);

        var loadIndex2Match = tireEntry.LoadIndex2 is not null || tireCode.LoadIndex2 is not null
            ? GetParameterMatch(tireCode.LoadIndex2?.ToString() ?? "",
                tireEntry.LoadIndex2?.ToString(CultureInfo.InvariantCulture) ?? "")
            : null;
        if (loadIndex2Match is not null)
            parameterMatches.Add(loadIndex2Match);

        var speedIndexMatch = GetParameterMatch(tireCode.SpeedRating ?? "",
            tireEntry.SpeedIndex ?? "");
        parameterMatches.Add(speedIndexMatch);

        var tireCodeString = tireCode.PostprocessedTireCode;
        var tireEntryString = tireEntry.GetTireCodeString();

        var totalDistance = parameterMatches.Sum(m => m.RequiredCharEdits);
        var estimatedAccuracy = GetAccuracyForLevenshteinDistance(totalDistance, tireCodeString, tireEntryString);
        var matchedMainParameterCount = parameterMatches
            .Take(4) // width, diameter, profile, construction 
            .Count(m => m.MatchesExactly);

        return new TireDbMatchDto(
            TireEntry: tireEntry,
            TotalRequiredCharEdits: totalDistance,
            EstimatedAccuracy: estimatedAccuracy,
            MatchedMainParameterCount: matchedMainParameterCount,
            WidthMatch: widthMatch,
            DiameterMatch: diameterMatch,
            ProfileMatch: profileMatch,
            ConstructionMatch: constructionMatch,
            LoadIndexMatch: loadIndexMatch,
            LoadIndex2Match: loadIndex2Match,
            SpeedIndexMatch: speedIndexMatch
        );
    }

    private ParameterMatchDto GetParameterMatch(string parameter1, string parameter2)
    {
        var levenshtein = new Levenshtein(parameter1);
        var distance = levenshtein.DistanceFrom(parameter2);
        var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, parameter1, parameter2);

        return new ParameterMatchDto(distance, estimatedAccuracy);
    }

    private decimal GetAccuracyForLevenshteinDistance(int distance, string string1, string string2)
    {
        var stringLength = Math.Max((decimal)string1.Length, (decimal)string2.Length);
        if (stringLength == 0)
            return 0;
        return 1 - distance / stringLength;
    }
}