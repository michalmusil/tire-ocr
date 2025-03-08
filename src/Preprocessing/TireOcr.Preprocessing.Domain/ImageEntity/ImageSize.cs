using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.ImageEntity;

public class ImageSize : ValueObject
{
    public int Height { get; }
    public int Width { get; }

    public ImageSize(int height, int width)
    {
        Height = height;
        Width = width;
    }


    protected override IEnumerable<object?> GetEqualityComponents() => [Height, Width];
}