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
            .ThenByDescending(x => x.tc.GetProcessedCode().Length)
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
        string cleanedUp = Regex.Replace(rawCode, @"[\n]", "\\");
        cleanedUp = cleanedUp
            .Replace("|", "/")
            .Replace(";", "")
            .Replace(":", "")
            .Replace(",", "")
            .Replace(".", "");
        cleanedUp = Regex.Replace(cleanedUp, @"\s+", "|");
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
        var widthCharCount = ExtractSectionWidth(tireCode, leftOfAnchor);
        leftOfAnchor = SubtractCharacters(leftOfAnchor, widthCharCount, fromStart: false);

        ExtractVehicleClass(tireCode, leftOfAnchor);

        var rightOfAnchor = tireCode.RawCode.Substring(tireCodeAnchorMatch.Index + tireCodeAnchorMatch.Length);
        var constructionAndDeprecatedSpeedRatingCharCount = ExtractConstructionAndDeprecatedSpeedRating(
            tireCode,
            rightOfAnchor
        );
        rightOfAnchor = SubtractCharacters(
            rightOfAnchor,
            constructionAndDeprecatedSpeedRatingCharCount,
            fromStart: true
        );

        var diameterCharCount = ExtractDiameter(tireCode, rightOfAnchor);
        rightOfAnchor = rightOfAnchor = SubtractCharacters(rightOfAnchor, diameterCharCount, fromStart: true);

        var rangeAndIndexCharCount = ExtractLoadRangeAndIndex(tireCode, rightOfAnchor);
        rightOfAnchor = rightOfAnchor = SubtractCharacters(rightOfAnchor, rangeAndIndexCharCount, fromStart: true);

        ExtractSpeedRating(tireCode, rightOfAnchor);

        return tireCode;
    }

    private int ExtractSectionWidth(TireCode code, string leftOfAspectRatio)
    {
        Match widthMatch = Regex.Match(leftOfAspectRatio, @"(?<Width>\d{3})\|?$");
        var width = widthMatch.Success ? widthMatch.Groups["Width"].Value : null;
        if (width != null)
        {
            var widthIsValid = int.TryParse(width, out var widthValue);
            if (widthIsValid)
                code.Width = widthValue;
        }

        return widthMatch.Length;
    }

    private int ExtractVehicleClass(TireCode code, string leftOfWidth)
    {
        Match classMatch = Regex.Match(leftOfWidth, @"[\\,|,^]+(?<VehicleClass>[ACLPST]{1,2})\|?$");
        var vehicleClass = classMatch.Success ? classMatch.Groups["VehicleClass"].Value : null;
        code.VehicleClass = vehicleClass;

        return classMatch.Length;
    }

    private int ExtractConstructionAndDeprecatedSpeedRating(TireCode code, string rightOfAspectRatio)
    {
        Match constructionMatch = Regex.Match(rightOfAspectRatio,
            @"^\|?(?<DeprecatedSpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})?(?<Construction>[RDB]{1})\|?");
        if (constructionMatch.Success)
        {
            code.Construction = constructionMatch.Groups["Construction"].Success
                ? constructionMatch.Groups["Construction"].Value
                : null;
            code.DeprecatedSpeedRating = constructionMatch.Groups["DeprecatedSpeedRating"].Success
                ? constructionMatch.Groups["DeprecatedSpeedRating"].Value
                : null;
        }

        return constructionMatch.Length;
    }

    private int ExtractDiameter(TireCode code, string leftOfConstruction)
    {
        Match diameterMatch = Regex.Match(leftOfConstruction, @"^(?<Diameter>\d{1,2})\|?");
        var diameter = diameterMatch.Success ? diameterMatch.Groups["Diameter"].Value : null;
        if (diameter is not null)
        {
            var diameterIsValid = int.TryParse(diameter, out var diameterValue);
            if (diameterIsValid)
                code.Diameter = diameterValue;
        }

        return diameterMatch.Length;
    }

    private int ExtractLoadRangeAndIndex(TireCode code, string leftOfDiameter)
    {
        Match loadRangeIndexMatch =
            Regex.Match(leftOfDiameter, @"^(?<LoadRangeIndex>([A-Z]{1,2}\d{1,3}/?)?\d{2,3})\|?");
        var loadRangeAndIndex = loadRangeIndexMatch.Success ? loadRangeIndexMatch.Groups["LoadRangeIndex"].Value : null;
        code.LoadRangeAndIndex = loadRangeAndIndex;

        return loadRangeIndexMatch.Length;
    }

    private int ExtractSpeedRating(TireCode code, string leftOfLoadIndex)
    {
        Match speedRatingMatch = Regex.Match(leftOfLoadIndex,
            @"^(?<SpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})\|?");
        var speedRating = speedRatingMatch.Success ? speedRatingMatch.Groups["SpeedRating"].Value : null;
        code.SpeedRating = speedRating;

        return speedRatingMatch.Length;
    }

    private string SubtractCharacters(string original, int numberOfChars, bool fromStart)
    {
        if (numberOfChars < 1 || numberOfChars > original.Length)
            return original;

        if (fromStart)
            return original.Substring(numberOfChars, original.Length - numberOfChars);

        return original.Substring(0, original.Length - numberOfChars);
    }
}