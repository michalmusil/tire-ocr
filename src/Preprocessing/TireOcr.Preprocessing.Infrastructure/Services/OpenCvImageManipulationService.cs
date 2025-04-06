using OpenCvSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class OpenCvImageManipulationService : IImageManipulationService
{
    public ImageSize GetRawImageSize(byte[] rawImage)
    {
        using var matImage = Mat.FromImageData(rawImage);
        return new ImageSize(matImage.Height, matImage.Width);
    }

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

        using var input = image.ToCv2().CvtColor(ColorConversionCodes.BGR2GRAY);
        using var output = new Mat();

        clahe.Apply(input, output);
        return output.ToDomain(image.Name);
    }

    public Image UnwrapRingIntoRectangle(Image image, ImageCoordinate center, double innerRadius, double outerRadius)
    {
        var inner = (int)innerRadius;
        var outer = (int)outerRadius;
        int croppedHeight = (int)(2 * Math.PI * innerRadius);

        using var input = image.ToCv2();
        using var fullPolar = new Mat();

        Cv2.WarpPolar(
            input,
            fullPolar,
            new Size(outer, croppedHeight),
            new Point2f(center.X, center.Y),
            outerRadius,
            interpolationFlags: InterpolationFlags.Cubic,
            warpPolarMode: WarpPolarMode.Linear
        );

        var tireThickness = outer - inner;
        using var croppedResult = new Mat(
            fullPolar,
            new Rect(
                location: new Point(X: (fullPolar.Width - tireThickness), Y: 0),
                size: new Size(width: tireThickness, height: croppedHeight)
            )
        );
        using var rotatedResult = new Mat();
        Cv2.Rotate(croppedResult, rotatedResult, RotateFlags.Rotate90Counterclockwise);

        return rotatedResult.ToDomain(image.Name);
    }

    public Image CopyAndAppendImagePortionFromLeft(Image image, double appendPortionWidthRatio)
    {
        var originalImage = image.ToCv2();

        var width = originalImage.Width;
        var height = originalImage.Height;
        var appendixWidth = (int)(width * appendPortionWidthRatio);

        if (appendixWidth <= 0)
            throw new ArgumentException("Image size and appendPortionWidthRatio must be greater than 0");

        var appendixRect = new Rect(0, 0, appendixWidth, height);

        using var leftStrip = new Mat(originalImage, appendixRect);
        using var resultImage = new Mat(height, width + appendixWidth, originalImage.Type());
        
        originalImage.CopyTo(new Mat(resultImage, new Rect(0, 0, width, height)));
        leftStrip.CopyTo(new Mat(resultImage, new Rect(width, 0, appendixWidth, height)));

        return resultImage.ToDomain(image.Name);
    }
}