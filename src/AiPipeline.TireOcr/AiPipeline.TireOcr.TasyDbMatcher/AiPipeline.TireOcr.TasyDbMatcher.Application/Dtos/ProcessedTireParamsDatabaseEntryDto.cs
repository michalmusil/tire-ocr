using System.Text.RegularExpressions;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public class ProcessedTireParamsDatabaseEntryDto
{
    public decimal Width { get; init; }
    public decimal Diameter { get; init; }
    public decimal Profile { get; init; }
    public string? Construction { get; init; }
    public int? LoadIndex { get; init; }
    public int? LoadIndex2 { get; init; }
    public string? SpeedIndex { get; init; }
    public string? LoadIndexSpeedIndex { get; init; }

    public ProcessedTireParamsDatabaseEntryDto(RawTireParamsDatabaseEntryDto rawParams)
    {
        var parsedWidth = decimal.TryParse(rawParams.ProductSizeWidth, out var width);
        Width = parsedWidth ? width : 0;
        var parsedDiameter = decimal.TryParse(rawParams.ProductSizeDiameter, out var diameter);
        Diameter = parsedDiameter ? diameter : 0;
        var parsedProfile = decimal.TryParse(rawParams.ProductSizeProfile, out var profile);
        Profile = parsedProfile ? profile : 0;
        var parsedLoadIndex = int.TryParse(rawParams.ProductLi, out var li);
        LoadIndex = parsedLoadIndex ? li : 0;

        if (rawParams.ProductLisi is not null)
        {
            LoadIndexSpeedIndex = rawParams.ProductLisi;
            var lisiParts = rawParams.ProductLisi.Split('/');
            if (lisiParts.Length == 2)
            {
                var match = Regex.Match(lisiParts[1], @"^\d{2,3}");
                var li2String = match.Success ? match.Value : null;
                var parsedLoadIndex2 = int.TryParse(li2String, out var li2);
                LoadIndex2 = parsedLoadIndex2 ? li2 : null;
            }
        }

        Construction = rawParams.ProductConstruction;
        if (rawParams.ProductSi is not null)
        {
            var curedSpeedIndex = rawParams.ProductSi
                .TrimStart('0')
                .Replace("_", string.Empty);
            SpeedIndex = curedSpeedIndex;
        }
    }

    public string GetTireCodeString()
    {
        var diameter = decimal.Round(Diameter, 1);

        return $"{Width}/{Profile}{Construction}{diameter}{LoadIndexSpeedIndex}";
    }
}