using System.Text;
using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class TireCodeValueObject : ValueObject
{
    public required string RawCode { get; init; }
    public string? VehicleClass { get; init; }
    public decimal? Width { get; init; }
    public decimal? AspectRatio { get; init; }
    public string? Construction { get; init; }
    public decimal? Diameter { get; init; }
    public string? LoadIndex { get; init; }
    public string? SpeedRating { get; init; }

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
            .Append(LoadIndex)
            .Append(SpeedRating);
        return builder.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents() =>
    [
        RawCode, VehicleClass, AspectRatio, Width, AspectRatio, Construction, Diameter, LoadIndex, SpeedRating
    ];
}