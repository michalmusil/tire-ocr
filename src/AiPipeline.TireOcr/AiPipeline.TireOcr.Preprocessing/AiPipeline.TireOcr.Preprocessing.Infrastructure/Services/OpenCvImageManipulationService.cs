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

    public Image ApplyBitwiseNot(Image image)
    {
        using var originalImage = image.ToCv2();
        using var invertedImage = new Mat();

        Cv2.BitwiseNot(originalImage, invertedImage);

        return invertedImage.ToDomain(image.Name);
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
        Size? tileGridSize = windowSize is null ? new Size(5, 5) : new Size(windowSize.Width, windowSize.Height);
        using var clahe = Cv2.CreateCLAHE(clipLimit, tileGridSize);

        using var input = image.ToCv2().CvtColor(ColorConversionCodes.BGR2GRAY);
        using var output = new Mat();

        clahe.Apply(input, output);
        return output.ToDomain(image.Name);
    }

    public Image ApplyNormalization(Image image, int minValue = 0, int maxValue = 255)
    {
        using var inputImage = image.ToCv2().CvtColor(ColorConversionCodes.BGR2GRAY);
        using var normalizedImage = new Mat();

        Cv2.Normalize(inputImage, normalizedImage, minValue, maxValue, NormTypes.MinMax);

        return normalizedImage.ToDomain(image.Name);
    }

    public Image ApplyBilateralFilter(Image image, int d, double sigmaColor, double sigmaSpace)
    {
        using var inputImage = image.ToCv2().CvtColor(ColorConversionCodes.BGR2GRAY);
        using var filteredImage = new Mat();

        Cv2.BilateralFilter(inputImage, filteredImage, d, sigmaColor, sigmaSpace);


        return filteredImage.ToDomain(image.Name);
    }

    public Image AddImagesWeighted(Image image1, double alpha, Image image2, double beta)
    {
        using var img1 = image1.ToCv2();
        using var img2 = image2.ToCv2();
        using var result = new Mat();
        Cv2.AddWeighted(img1, alpha, img2, beta, 0, result);
        return result.ToDomain(image1.Name);
    }

    public Image ApplyGausianBlur(Image image, int kernelWidth = 5, int kernelHeight = 5, int sigmaX = 0,
        int sigmaY = 0)
    {
        using var inputImage = image.ToCv2();
        using var blurredImage = new Mat();

        var kernelSize = new Size(kernelWidth, kernelHeight);

        Cv2.GaussianBlur(inputImage, blurredImage, kernelSize, sigmaX, sigmaY);

        return blurredImage.ToDomain(image.Name);
    }

    public Image ApplyCannyEdgeDetection(Image image, double treshold1 = 100, double treshold2 = 200)
    {
        using var inputImage = image.ToCv2();
        using var edgesImage = new Mat();

        Cv2.Canny(inputImage, edgesImage, treshold1, treshold2);

        return edgesImage.ToDomain(image.Name);
    }

    public Image ApplySobelEdgeDetection(Image image, bool preBlur)
    {
        using var inputImage = ApplyGrayscale(image).ToCv2();
        if (preBlur)
        {
            Cv2.GaussianBlur(inputImage, inputImage, new Size(3, 3), 0);
        }

        using var gradX = new Mat();
        using var gradY = new Mat();
        Cv2.Sobel(inputImage, gradX, MatType.CV_16S, 1, 0, ksize: 3);
        Cv2.Sobel(inputImage, gradY, MatType.CV_16S, 0, 1, ksize: 3);

        using var absX = new Mat();
        using var absY = new Mat();
        Cv2.ConvertScaleAbs(gradX, absX);
        Cv2.ConvertScaleAbs(gradY, absY);

        using var result = new Mat();
        Cv2.AddWeighted(absX, 0.5, absY, 0.5, 0, result);

        return result.ToDomain(image.Name);
    }

    public Image ApplySharpening(Image image, float strength = 5)
    {
        using var inputImage = image.ToCv2();
        using var sharpenedImage = new Mat();
        var lowering = -1f;

        using var kernel = new Mat(3, 3, MatType.CV_32F);
        kernel.SetArray(new float[]
        {
            0, lowering, 0,
            lowering, strength, lowering,
            0, lowering, 0
        });

        Cv2.Filter2D(inputImage, sharpenedImage, -1, kernel);

        return sharpenedImage.ToDomain(image.Name);
    }

    public Image ApplyAdaptiveTreshold(Image image, int value = 255, int blockSize = 11, double c = 0)
    {
        using var inputImage = image.ToCv2().CvtColor(ColorConversionCodes.BGR2GRAY);
        using var tresholdedImage = new Mat();

        Cv2.AdaptiveThreshold(inputImage, tresholdedImage, value,
            AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, blockSize, c);

        return tresholdedImage.ToDomain(image.Name);
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

    public BoundingBox GetBoundingBoxForTireCodeString(Image image, StringInImage stringInImage)
    {
        using var originalImage = image.ToCv2();
        if (stringInImage.Characters.Count < 1)
            throw new ArgumentException("String in image must contain at least one character");
        var topLeftX = stringInImage.Characters
            .MinBy(c => c.TopLeftCoordinate.X)!.TopLeftCoordinate.X;
        var topLeftY = stringInImage.Characters
            .MinBy(c => c.TopLeftCoordinate.Y)!.TopLeftCoordinate.Y;

        var bottomRightX = stringInImage.Characters
            .MaxBy(c => c.BottomRightCoordinate.X)!.BottomRightCoordinate.X;
        var bottomRightY = stringInImage.Characters
            .MaxBy(c => c.BottomRightCoordinate.Y)!.BottomRightCoordinate.Y;

        var widthCorrection = (int)(originalImage.Width * 0.1);
        topLeftX = Math.Max(0, topLeftX - widthCorrection);
        bottomRightX = Math.Min(originalImage.Width, bottomRightX + widthCorrection);

        var bboxHeight = bottomRightY - topLeftY;
        var heightCorrection = (int)(bboxHeight * 0.3);
        topLeftY = Math.Max(0, topLeftY - heightCorrection);
        bottomRightY = Math.Min(originalImage.Height, bottomRightY + heightCorrection);

        return new BoundingBox
        {
            TopLeft = new ImageCoordinate(topLeftX, topLeftY),
            BottomRight = new ImageCoordinate(bottomRightX, bottomRightY),
        };
    }

    public Image ApplyMaskToEverythingExceptBoundingBoxes(Image image, IEnumerable<BoundingBox> boundingBoxes)
    {
        using var originalImage = image.ToCv2();
        using var onlyBoundingBoxes = new Mat(originalImage.Size(), originalImage.Type(), new Scalar(255, 255, 255));

        foreach (var bBox in boundingBoxes)
        {
            if (CoordinateIsValid(bBox.TopLeft, originalImage.Width, originalImage.Height) &&
                CoordinateIsValid(bBox.BottomRight, originalImage.Width, originalImage.Height))
            {
                using var roiOriginal = new Mat(originalImage, bBox.ToRect());
                using var roiCanvas = new Mat(onlyBoundingBoxes, bBox.ToRect());
                roiOriginal.CopyTo(roiCanvas);
            }
        }

        return onlyBoundingBoxes.ToDomain(image.Name);
    }

    public Image? StackImagesVertically(List<Image> images)
    {
        if (images.Count < 1)
            return null;

        var cv2Images = images
            .Select(image => image.ToCv2())
            .ToArray();

        var width = cv2Images.Max(i => i.Width);
        var height = cv2Images.Sum(i => i.Height);
        var type = cv2Images.First().Type();
        if (cv2Images.Any(i => i.Type() != type))
            return null;

        using var resultImage = new Mat(height, width, type);
        var yOffset = 0;
        for (int i = 0; i < cv2Images.Length; i++)
        {
            var previousImage = i == 0 ? null : cv2Images[i - 1];
            var currentImage = cv2Images[i];

            currentImage.CopyTo(new Mat(resultImage, new Rect(0, yOffset, currentImage.Width, currentImage.Height)));
            yOffset += currentImage.Height;
        }

        foreach (var image in cv2Images)
            image.Dispose();

        return resultImage.ToDomain(images.First().Name);
    }

    private bool CoordinateIsValid(ImageCoordinate coordinate, int imageWidth, int imageHeight) => coordinate.X >= 0 &&
        coordinate.Y >= 0 && coordinate.X <= imageWidth && coordinate.Y <= imageHeight;
}