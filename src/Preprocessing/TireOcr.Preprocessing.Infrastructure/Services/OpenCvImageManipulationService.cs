using OpenCvSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class OpenCvImageManipulationService : IImageManipulationService
{
    public Image ResizeToMaxSideSize(Image image, int maxSide)
    {
        var height = image.Size.Height;
        var width = image.Size.Width;

        var largerSide = Math.Max(height, width);
        if (largerSide <= maxSide)
            return image;

        var scaleFactor = (double)maxSide / largerSide;
        var newWidth = (int)(width * scaleFactor);
        var newHeight = (int)(height * scaleFactor);

        using var inputImage = image.ToCv2();
        using var resizedImage = new Mat();
        Cv2.Resize(inputImage, resizedImage, new Size(newWidth, newHeight));

        return resizedImage.ToDomain(image.Name);
    }

    public Image ApplyGrayscale(Image image)
    {
        using var inputImage = image.ToCv2();
        using var grayImage = new Mat();

        Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);
        return grayImage.ToDomain(image.Name);
    }

    public Image ApplyClahe(Image image, double clipLimit = 40.0, ImageSize? windowSize = null)
    {
        Size? tileGridSize = windowSize is null ? null : new Size(windowSize.Width, windowSize.Height);
        using var clahe = Cv2.CreateCLAHE(clipLimit, tileGridSize);

        using var input = image.ToCv2();
        using var output = new Mat();

        clahe.Apply(input, output);
        return output.ToDomain(image.Name);
    }

    public Image UnwrapRingIntoRectangle(Image image, ImageCoordinate center, double innerRadius, double outerRadius)
    {
        var inner = (int)innerRadius;
        var outer = (int)outerRadius;
        int finalWidth = (int)(2 * Math.PI * innerRadius);

        using var input = image.ToCv2();
        using var fullPolar = new Mat();

        Cv2.WarpPolar(
            input,
            fullPolar,
            new Size(finalWidth, outer),
            new Point2f(center.X, center.Y),
            outerRadius,
            interpolationFlags: InterpolationFlags.Nearest | InterpolationFlags.Cubic,
            warpPolarMode: WarpPolarMode.Linear
        );

        using var croppedResult = new Mat(
            fullPolar,
            new Rect(0, (int)innerRadius, fullPolar.Width, fullPolar.Height - (int)innerRadius)
        );
        
        using var rotatedResult = new Mat();
        Cv2.Rotate(croppedResult, rotatedResult, RotateFlags.Rotate90Clockwise);
        
        return rotatedResult.ToDomain(image.Name);
    }
}