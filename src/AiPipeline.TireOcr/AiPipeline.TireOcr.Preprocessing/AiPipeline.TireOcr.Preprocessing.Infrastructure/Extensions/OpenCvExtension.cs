using OpenCvSharp;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Infrastructure.Extensions;

public static class OpenCvExtension
{
    public static Image ToDomain(this Mat mat, string name)
    {
        var data = mat.ToBytes();
        var size = new ImageSize(mat.Height, mat.Width);
        return new Image(data, name, size);
    }

    public static Mat ToCv2(this Image image)
    {
        return Mat.FromImageData(image.Data);
    }

    public static Rect ToRect(this BoundingBox boundingBox)
    {
        return new Rect(
            X: boundingBox.TopLeft.X,
            Y: boundingBox.TopLeft.Y,
            Width: boundingBox.BottomRight.X - boundingBox.TopLeft.X,
            Height: boundingBox.BottomRight.Y - boundingBox.TopLeft.Y
        );
    }
}