using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class CircleInImage : ValueObject
{
    public ImageCoordinate Center { get; }
    public double InnerRadius { get; }
    public double OuterRadius { get; }

    public CircleInImage(ImageCoordinate center, double innerRadius, double outerRadius)
    {
        Center = center;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
    }

    protected override IEnumerable<object?> GetEqualityComponents() => [Center, InnerRadius, OuterRadius];
}