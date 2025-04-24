using System.Text;

namespace TireOcr.Postprocessing.Domain.TireCodeEntity;

public class TireCode
{
    public required string RawCode { get; init; }

    public string? VehicleClass { get; set; }
    public int? Width { get; set; }
    public int? AspectRatio { get; set; }
    public string? DeprecatedSpeedRating { get; set; }
    public string? Construction { get; set; }
    public int? Diameter { get; set; }
    public string? LoadRangeAndIndex { get; set; }
    public string? SpeedRating { get; set; }

    public bool WasProcessedSuccessfully => VehicleClass is not null || Width is not null || AspectRatio is not null ||
                                            Construction is not null || Diameter is not null ||
                                            LoadRangeAndIndex is not null ||
                                            SpeedRating is not null;

    public string GetProcessedCode()
    {
        var builder = new StringBuilder()
            .Append(VehicleClass)
            .Append(Width);

        if (Width.HasValue || AspectRatio.HasValue)
            builder
                .Append('/');

        builder
            .Append(AspectRatio)
            .Append(DeprecatedSpeedRating)
            .Append(Construction)
            .Append(Diameter)
            .Append(LoadRangeAndIndex)
            .Append(SpeedRating);
        return builder.ToString();
    }
}