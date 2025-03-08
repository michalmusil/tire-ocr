using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class ImageCoordinate : ValueObject
{
    public int X { get; }
    public int Y { get; }

    public ImageCoordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    protected override IEnumerable<object?> GetEqualityComponents() => [X, Y];
}