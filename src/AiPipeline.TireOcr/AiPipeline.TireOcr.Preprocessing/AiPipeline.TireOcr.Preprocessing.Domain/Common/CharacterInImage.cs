using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class CharacterInImage : ValueObject
{
    public required ImageCoordinate TopLeftCoordinate { get; init; }
    public required ImageCoordinate BottomRightCoordinate { get; init; }
    public required char Character { get; init; }

    public bool IsWithinVerticalSpanOf(CharacterInImage other)
    {
        var top = TopLeftCoordinate.Y;
        var bottom = BottomRightCoordinate.Y;
        var otherTop = other.TopLeftCoordinate.Y;
        var otherBottom = other.BottomRightCoordinate.Y;

        return (top <= otherTop && otherTop <= bottom) ||
               (top <= otherBottom && otherBottom <= bottom) ||
               (otherTop <= top && top <= otherBottom) ||
               (otherTop <= bottom && bottom <= otherBottom);
    }

    public bool IsLeftNeighborOf(CharacterInImage other)
    {
        var right = BottomRightCoordinate.X;
        var left = TopLeftCoordinate.X;
        var width = right - left;

        var otherLeft = other.TopLeftCoordinate.X;

        return otherLeft > left && otherLeft <= right + 4 * width;
    }

    protected override IEnumerable<object?> GetEqualityComponents() =>
        [TopLeftCoordinate, BottomRightCoordinate, Character];
}