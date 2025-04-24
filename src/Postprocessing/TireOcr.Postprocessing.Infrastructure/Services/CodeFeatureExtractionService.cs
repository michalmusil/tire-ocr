using System.Reflection;
using System.Text.RegularExpressions;
using TireOcr.Postprocessing.Application.Services;
using TireOcr.Postprocessing.Domain.TireCodeEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Postprocessing.Infrastructure.Services;

public class CodeFeatureExtractionService : ICodeFeatureExtractionService
{
    public DataResult<IEnumerable<TireCode>> ExtractTireCodes(string rawTireCodeValue)
    {
        var cleanedUpTireCode = PerformPreMatchingCleanup(rawTireCodeValue);
        var anchors = GetAnchorMatches(cleanedUpTireCode).ToList();

        if (!anchors.Any())
            return DataResult<IEnumerable<TireCode>>.NotFound("No tire code was detected during Postprocessing");

        var validTireCodes = anchors
            .Select(a => ProcessPotentialTireCode(a, cleanedUpTireCode))
            .Where(tc => tc.WasProcessedSuccessfully)
            .ToList();

        if (!validTireCodes.Any())
            return DataResult<IEnumerable<TireCode>>.NotFound("No valid tire code was detected during Postprocessing");

        return DataResult<IEnumerable<TireCode>>.Success(validTireCodes);
    }

    public DataResult<TireCode> PickBestMatchingTireCode(IEnumerable<TireCode> tireCodes)
    {
        var bestTireCode = tireCodes
            .Select(tc => (tc, GetScoreOfTireCode(tc)))
            .OrderByDescending(x => x.Item2)
            .ThenBy(x => x.tc.GetProcessedCode().Length)
            .FirstOrDefault().tc;

        if (bestTireCode is null)
            return DataResult<TireCode>.Failure(new Failure(500,
                "Tire code scoring called with empty enumerable of tire codes"));

        return DataResult<TireCode>.Success(bestTireCode);
    }

    private int GetScoreOfTireCode(TireCode tireCode)
    {
        var score = 0;

        var properties = typeof(TireCode).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (!property.CanRead)
                continue;

            var value = property.GetValue(tireCode);
            if (value is not null)
                score++;
        }

        return score;
    }

    private string PerformPreMatchingCleanup(string rawCode)
    {
        var cleanedUp = Regex.Replace(rawCode, @"[\n\s.,]", "");
        cleanedUp = cleanedUp.Replace("|", "/").Replace(":", "/");
        cleanedUp = cleanedUp.ToUpperInvariant();

        return cleanedUp;
    }

    private IEnumerable<Match> GetAnchorMatches(string code)
    {
        var regex = new Regex(@"/(?<AspectRatio>\d{2,3})");
        return regex.Matches(code);
    }

    private TireCode ProcessPotentialTireCode(Match tireCodeAnchorMatch, string code)
    {
        var tireCode = new TireCode
        {
            RawCode = code,
        };

        var aspectRatioValid = int.TryParse(tireCodeAnchorMatch.Groups["AspectRatio"].Value, out var aspectRatio);
        if (!aspectRatioValid)
            return tireCode;

        tireCode.AspectRatio = aspectRatio;

        var leftOfAnchor = tireCode.RawCode.Substring(0, tireCodeAnchorMatch.Index);
        ExtractSectionWidth(tireCode, leftOfAnchor);
        leftOfAnchor = GetSubstringWithoutEnd(leftOfAnchor, tireCode.Width?.ToString());

        ExtractVehicleClass(tireCode, leftOfAnchor);

        var rightOfAnchor = tireCode.RawCode.Substring(tireCodeAnchorMatch.Index + tireCodeAnchorMatch.Length);
        ExtractConstructionAndDeprecatedSpeedRating(tireCode, rightOfAnchor);
        rightOfAnchor = GetSubstringWithoutBeginning(rightOfAnchor, tireCode.Construction);

        ExtractDiameter(tireCode, rightOfAnchor);
        rightOfAnchor = GetSubstringWithoutBeginning(rightOfAnchor, tireCode.Diameter?.ToString());

        ExtractLoadRangeAndIndex(tireCode, rightOfAnchor);
        rightOfAnchor = GetSubstringWithoutBeginning(rightOfAnchor, tireCode.LoadRangeAndIndex);

        ExtractSpeedRating(tireCode, rightOfAnchor);

        return tireCode;
    }

    private void ExtractSectionWidth(TireCode code, string leftOfAspectRatio)
    {
        Match widthMatch = Regex.Match(leftOfAspectRatio, @"(?<Width>\d{3})$");
        var width = widthMatch.Success ? widthMatch.Groups["Width"].Value : null;
        if (width != null)
        {
            var widthIsValid = int.TryParse(width, out var widthValue);
            if (widthIsValid)
                code.Width = widthValue;
        }
    }

    private void ExtractVehicleClass(TireCode code, string leftOfWidth)
    {
        Match classMatch = Regex.Match(leftOfWidth, @"(?<VehicleClass>[ACLPST]{1,2})$");
        var vehicleClass = classMatch.Success ? classMatch.Groups["VehicleClass"].Value : null;
        code.VehicleClass = vehicleClass;
    }

    private void ExtractConstructionAndDeprecatedSpeedRating(TireCode code, string rightOfAspectRatio)
    {
        Match constructionMatch = Regex.Match(rightOfAspectRatio,
            @"^(?<DeprecatedSpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})?(?<Construction>[RDB]{1})");
        if (constructionMatch.Success)
        {
            code.Construction = constructionMatch.Groups["Construction"].Success
                ? constructionMatch.Groups["Construction"].Value
                : null;
            code.DeprecatedSpeedRating = constructionMatch.Groups["DeprecatedSpeedRating"].Success
                ? constructionMatch.Groups["DeprecatedSpeedRating"].Value
                : null;
        }
    }

    private void ExtractDiameter(TireCode code, string leftOfConstruction)
    {
        Match diameterMatch = Regex.Match(leftOfConstruction, @"^(?<Diameter>\d{1,3})");
        var diameter = diameterMatch.Success ? diameterMatch.Groups["Diameter"].Value : null;
        if (diameter is not null)
        {
            var diameterIsValid = int.TryParse(diameter, out var diameterValue);
            if (diameterIsValid)
                code.Diameter = diameterValue;
        }
    }

    private void ExtractLoadRangeAndIndex(TireCode code, string leftOfDiameter)
    {
        Match loadRangeIndexMatch = Regex.Match(leftOfDiameter, @"^(?<LoadRangeIndex>([A-Z]{1,2}\d{1,3}/?)?\d{2,3})");
        var loadRangeAndIndex = loadRangeIndexMatch.Success ? loadRangeIndexMatch.Groups["LoadRangeIndex"].Value : null;
        code.LoadRangeAndIndex = loadRangeAndIndex;
    }

    private void ExtractSpeedRating(TireCode code, string leftOfLoadIndex)
    {
        Match speedRatingMatch = Regex.Match(leftOfLoadIndex,
            @"^(?<SpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})");
        var speedRating = speedRatingMatch.Success ? speedRatingMatch.Groups["SpeedRating"].Value : null;
        code.SpeedRating = speedRating;
    }

    private string GetSubstringWithoutEnd(string fullString, string? end)
    {
        if (end == null)
            return fullString;

        if (end.Length >= fullString.Length)
            return fullString;

        return fullString.Substring(0, fullString.Length - end.Length);
    }

    private string GetSubstringWithoutBeginning(string fullString, string? beginning)
    {
        if (beginning == null)
            return fullString;

        if (beginning.Length >= fullString.Length)
            return fullString;

        return fullString.Substring(beginning.Length, fullString.Length - beginning.Length);
    }
}