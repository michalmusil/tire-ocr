using System.Text;
using System.Text.RegularExpressions;
using TireOcr.Shared.Domain;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class TireCodeValueObject : ValueObject
{
    public string RawCode { get; private set; }
    public string? VehicleClass { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? AspectRatio { get; private set; }
    public string? Construction { get; private set; }
    public decimal? Diameter { get; private set; }
    public char? LoadRange { get; private set; }
    public int? LoadIndex { get; private set; }
    public int? LoadIndex2 { get; private set; }
    public string? SpeedRating { get; private set; }

    public TireCodeValueObject(string rawCode, string? vehicleClass = null, decimal? width = null,
        decimal? aspectRatio = null, string? construction = null, decimal? diameter = null, char? loadRange = null,
        int? loadIndex = null, int? loadIndex2 = null, string? speedRating = null)
    {
        RawCode = rawCode;
        VehicleClass = vehicleClass;
        Width = width;
        AspectRatio = aspectRatio;
        Construction = construction;
        Diameter = diameter;
        LoadRange = loadRange;
        LoadIndex = loadIndex;
        LoadIndex2 = loadIndex2;
        SpeedRating = speedRating;
    }

    public override string ToString()
    {
        var builder = new StringBuilder()
            .Append(VehicleClass)
            .Append(Width);

        if (Width.HasValue || AspectRatio.HasValue)
            builder
                .Append('/');

        builder
            .Append(AspectRatio)
            .Append(Construction)
            .Append(Diameter)
            .Append(LoadRange)
            .Append(LoadIndex);
        if (LoadIndex2 is not null)
            builder
                .Append('/')
                .Append(LoadIndex2);

        builder.Append(SpeedRating);
        return builder.ToString();
    }

    /// <summary>
    /// Extracts tire code from a predefined label string. Label string is a modified classic tire code (e.g.
    /// "LT 215/70R20 109/102T"), but spaces are replaced with '_' and slashes are replaced with "-", so the
    /// previously mentioned tire code in label string format would be: "LT_215-70R20_109-102T". 
    /// </summary>
    /// <param name="labelString">Label string in correct format described above</param>
    /// <returns>A DataResult containing parsed TireCodeDto if parsing is successful, failure otherwise.</returns>
    public static DataResult<TireCodeValueObject> FromLabelString(string labelString)
    {
        var failureResult = DataResult<TireCodeValueObject>.Invalid(
            $"Label string '{labelString}' is nod in valid format. Valid format is for example \"LT_215-70R20_109-102T\", which translates to \"LT 215/70R20 109/102T\"."
        );

        var parts = labelString.Split('_');
        if (parts.Length < 1)
            return failureResult;

        var tireCode = new TireCodeValueObject(rawCode: labelString);

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var isFirst = i == 0;

            var isVehicleClassPart = isFirst && part.Length < 3 && part.All(char.IsLetter);
            if (isVehicleClassPart)
            {
                ParseLabelVehicleClass(tireCode, part);
                continue;
            }

            var dimensionsPartRegexMatch = Regex.Match(
                input: part,
                pattern:
                @"^(?<Width>\d{3}|\d{1,2}\.\d{1,2})-(?<AspectRatio>\d{2,3}|\d{1,2}\.\d{1,2})(?<DeprecatedSpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})?(?<Construction>[RDB]{1})(?<Diameter>\d{1,3}|\d{1,2}\.\d{1,2})");
            var isDimensionsPart = dimensionsPartRegexMatch.Success;
            if (isDimensionsPart)
            {
                ParseLabelDimensionsPart(tireCode, dimensionsPartRegexMatch);
                continue;
            }

            var loadAndSpeedPartRegexMatch = Regex.Match(
                input: part,
                pattern:
                @"^(?<LoadRange>[A-Z]{1})?(?<LoadIndex>(\d{1,3}-?)?\d{2,3})(?<SpeedRating>[A,B,C,D,E,F,G,J,K,L,M,N,P,Q,R,S,T,U,H,V,Z,W,Y]{1})");
            var isLoadAndSpeedPart = loadAndSpeedPartRegexMatch.Success;
            if (isLoadAndSpeedPart)
                ParseLoadAndSpeedPart(tireCode, loadAndSpeedPartRegexMatch);
        }

        var isValid = tireCode is
            { Width: not null, AspectRatio: not null, Diameter: not null, Construction: not null };

        if (!isValid)
            return DataResult<TireCodeValueObject>.Invalid($"Tire code '{labelString}' could not be parsed.");

        return DataResult<TireCodeValueObject>.Success(tireCode);
    }

    private static void ParseLabelVehicleClass(TireCodeValueObject tireCode, string vehicleClassPart) =>
        tireCode.VehicleClass = vehicleClassPart;

    private static void ParseLabelDimensionsPart(TireCodeValueObject tireCode, Match dimensionsPartMatch)
    {
        var hasWidth = decimal.TryParse(dimensionsPartMatch.Groups["Width"].Value, out var width);
        if (hasWidth)
            tireCode.Width = width;

        var hasAspectRatio = decimal.TryParse(dimensionsPartMatch.Groups["AspectRatio"].Value, out var aspectRatio);
        if (hasAspectRatio)
            tireCode.AspectRatio = aspectRatio;

        var hasConstruction = dimensionsPartMatch.Groups["Construction"].Success;
        if (hasConstruction)
            tireCode.Construction = dimensionsPartMatch.Groups["Construction"].Value;

        var hasDiameter = decimal.TryParse(dimensionsPartMatch.Groups["Diameter"].Value, out var diameter);
        if (hasDiameter)
            tireCode.Diameter = diameter;
    }

    private static void ParseLoadAndSpeedPart(TireCodeValueObject tireCode, Match lsPartMatch)
    {
        var hasLoadRange = lsPartMatch.Groups["LoadRange"].Success;
        if (hasLoadRange)
            tireCode.LoadRange = lsPartMatch.Groups["LoadRange"].Value.FirstOrDefault();

        var hasLoadIndex = lsPartMatch.Groups["LoadIndex"].Success;
        if (hasLoadIndex)
        {
            var loadIndexParts = lsPartMatch.Groups["LoadIndex"].Value.Split('-');
            var firstLoadIndexValid = int.TryParse(loadIndexParts[0], out var li1);
            if (firstLoadIndexValid)
                tireCode.LoadIndex = li1;

            if (loadIndexParts.Length > 1 && int.TryParse(loadIndexParts[1], out var li2))
                tireCode.LoadIndex2 = li2;
        }

        var hasSpeedRating = lsPartMatch.Groups["SpeedRating"].Success;
        if (hasSpeedRating)
            tireCode.SpeedRating = lsPartMatch.Groups["SpeedRating"].Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents() =>
    [
        RawCode, VehicleClass, AspectRatio, Width, AspectRatio, Construction, Diameter, LoadIndex, SpeedRating
    ];
}