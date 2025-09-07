namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public class ProcessedTireParamsDatabaseEntryDto
{
    public int Width { get; init; }
    public double Diameter { get; init; }
    public int Profile { get; init; }
    public required string Construction { get; init; }
    public int? LoadIndex { get; init; }
    public string? SpeedIndex { get; init; }
    public string? LoadIndexSpeedIndex { get; init; }

    public ProcessedTireParamsDatabaseEntryDto(RawTireParamsDatabaseEntryDto rawParams)
    {
        var parsedWidth = int.TryParse(rawParams.ProductSizeWidth, out var width);
        Width = parsedWidth ? width : 0;
        var parsedDiameter = double.TryParse(rawParams.ProductSizeDiameter, out var diameter);
        Diameter = parsedDiameter ? diameter : 0;
        var parsedProfile = int.TryParse(rawParams.ProductSizeProfile, out var profile);
        Profile = parsedProfile ? profile : 0;
        var parsedLoadIndex = int.TryParse(rawParams.ProductLi, out var li);
        LoadIndex = parsedLoadIndex ? li : 0;

        Construction = rawParams.ProductConstruction;

        if (rawParams.ProductSi is not null)
        {
            var curedSpeedIndex = rawParams.ProductSi
                .TrimStart('0')
                .Replace("_", string.Empty);
            SpeedIndex = curedSpeedIndex;
        }

        if (rawParams.ProductLisi is not null)
            LoadIndexSpeedIndex = rawParams.ProductLisi;
    }

    public string GetTireCodeString()
    {
        var diameter = double.Round(Diameter, 1);

        return $"{Width}/{Profile}{Construction}{diameter}{LoadIndexSpeedIndex}";
    }
}