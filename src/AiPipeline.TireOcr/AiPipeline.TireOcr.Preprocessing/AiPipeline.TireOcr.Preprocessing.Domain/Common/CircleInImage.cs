using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class CircleInImage : ValueObject
{
    public required ImageCoordinate Center { get; init; }
    public required double Radius { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() => [Center, Radius];
}