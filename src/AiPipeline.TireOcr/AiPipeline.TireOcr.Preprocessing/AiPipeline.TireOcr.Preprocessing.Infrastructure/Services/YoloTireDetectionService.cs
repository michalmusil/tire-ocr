using System.Diagnostics;
using OpenCvSharp;
using SkiaSharp;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;
using TireOcr.Shared.Result;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class YoloTireDetectionService : ITireDetectionService
{
    private const string RimClassName = "rim";
    private const double ConfidenceThreshold = 0.6;

    private readonly IMlModelResolverService _modelResolverService;

    public YoloTireDetectionService(IMlModelResolverService modelResolverService)
    {
        _modelResolverService = modelResolverService;
    }

    public async Task<DataResult<TireDetectionResultDto>> DetectTireRimCircle(Image image)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var modelResult = await _modelResolverService.Resolve<ITireDetectionService>();
        if (modelResult.IsFailure)
            return DataResult<TireDetectionResultDto>.Failure(
                new Failure(500, modelResult.PrimaryFailure?.Message ?? "Failed to resolve ML model")
            );

        var model = modelResult.Data!;
        using var yolo = new Yolo(new YoloOptions
        {
            OnnxModel = model.GetAbsolutePath(),
            ModelType = ModelType.Segmentation,
            Cuda = false,
            PrimeGpu = false
        });

        // Running through segmentation to get masks for the rim (wheel excl. the tire sidewall) and the entire wheel (incl. tire sidewall)
        using var imageToDetect = SKImage.FromBitmap(SKBitmap.Decode(image.Data));
        var results = yolo.RunSegmentation(imageToDetect);
        var rimMaskResult = results.FirstOrDefault(res =>
            string.Equals(res.Label.Name, RimClassName, StringComparison.CurrentCultureIgnoreCase));
        if (rimMaskResult is null || rimMaskResult.Confidence < ConfidenceThreshold)
            return DataResult<TireDetectionResultDto>.NotFound("No tire rim was detected in the image.");

        // If rim is found, I get a mask for it and get it's contour
        using var mask = ConvertSegmentationMaskToMat(
                detectedImage: imageToDetect,
                segmentationResult: rimMaskResult
            )
            .Threshold(1, 255, ThresholdTypes.Binary);
        using var invertedMask = new Mat();
        Cv2.BitwiseNot(mask, invertedMask);
        invertedMask.FindContours(out var contours, out _,
            RetrievalModes.External,
            ContourApproximationModes.ApproxSimple
        );
        var largestContour = contours.MaxBy(p => p.Length);
        if (largestContour is null)
            return DataResult<TireDetectionResultDto>.NotFound("No tire rim was detected in the image.");

        // Once I have the contour, I find an enclosing circle of the rim. The circle should be the border between rim and the tire sidewall
        Cv2.MinEnclosingCircle(largestContour, out var center, out var radius);
        var rimCircle = new CircleInImage
        {
            Center = new ImageCoordinate((int)center.X, (int)center.Y),
            Radius = radius
        };
        if (!IsCircleWithinImageBounds(rimCircle, image.Size)) 
            // If circle is not within original image bounds, the photo doesn't contain the entire wheel 
            return DataResult<TireDetectionResultDto>.Invalid("Detected rim circle is not within image bounds.");

        stopWatch.Stop();
        var timeTaken = stopWatch.Elapsed;
        var result = new TireDetectionResultDto(rimCircle, timeTaken);

        return DataResult<TireDetectionResultDto>.Success(result);
    }

    private Mat ConvertSegmentationMaskToMat(SKImage detectedImage, Segmentation segmentationResult)
    {
        var height = detectedImage.Height;
        var width = detectedImage.Width;

        var rimMask = new Mat(height, width, MatType.CV_8UC1, Scalar.White);
        foreach (var pixel in segmentationResult.SegmentedPixels)
        {
            rimMask.Set(pixel.Y, pixel.X, 0);
        }

        return rimMask;
    }

    private bool IsCircleWithinImageBounds(CircleInImage circle, ImageSize imageDimensions)
    {
        var circleMinX = circle.Center.X - circle.Radius;
        var circleMaxX = circle.Center.X + circle.Radius;

        var circleMinY = circle.Center.Y - circle.Radius;
        var circleMaxY = circle.Center.Y + circle.Radius;

        var isWithinHorizontalBounds = circleMinX > 0 && circleMaxX <= imageDimensions.Width;
        var isWithinVerticalBounds = circleMinY > 0 && circleMaxY <= imageDimensions.Height;
        return isWithinHorizontalBounds && isWithinVerticalBounds;
    }
}