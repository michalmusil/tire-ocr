using OpenCvSharp;

namespace TireOcr.Preprocessing.Infrastructure.Models;

public record LetterboxedImage(
    Mat Image,
    int ImageUpscaledWidth,
    int ImageUpscaledHeight,
    int OffsetX,
    int OffsetY
): IDisposable
{
    public void Dispose()
    {
        Image.Dispose();
    }
}