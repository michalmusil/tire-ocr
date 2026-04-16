using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class BoundingBox : ValueObject
{
    public required ImageCoordinate TopLeft { get; init; }
    public required ImageCoordinate BottomRight { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [TopLeft, BottomRight];
}