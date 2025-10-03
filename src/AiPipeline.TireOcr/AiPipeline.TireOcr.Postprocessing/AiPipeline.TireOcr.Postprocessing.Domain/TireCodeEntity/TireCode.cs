using System.Text;

namespace TireOcr.Postprocessing.Domain.TireCodeEntity;

public class TireCode
{
    public required string RawCode { get; init; }

    public string? VehicleClass { get; set; }
    public decimal? Width { get; set; }
    public decimal? AspectRatio { get; set; }
    public string? DeprecatedSpeedRating { get; set; }
    public string? Construction { get; set; }
    public decimal? Diameter { get; set; }

    public char? LoadRange { get; set; }

    // Single load index in passenger cars, single tire load index on Light Trucks
    public int? LoadIndex { get; set; }

    // Double tire load index. Only sometimes present on light trucks, which can have 2 sets of rear wheels.
    public int? LoadIndex2 { get; set; }
    public string? SpeedRating { get; set; }

    public bool WasProcessedSuccessfully => VehicleClass is not null || Width is not null || AspectRatio is not null ||
                                            Construction is not null || Diameter is not null || LoadRange is not null ||
                                            LoadIndex is not null || LoadIndex2 is not null || SpeedRating is not null;

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
            .Append(LoadRange)
            .Append(LoadIndex);

        if (LoadIndex2.HasValue)
            builder
                .Append('/')
                .Append(LoadIndex2);

        builder.Append(SpeedRating);
        return builder.ToString();
    }
}