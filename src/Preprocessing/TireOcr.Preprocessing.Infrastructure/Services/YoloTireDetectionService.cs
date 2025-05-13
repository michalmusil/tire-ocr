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

    private readonly IMlModelResolver _modelResolver;

    public YoloTireDetectionService(IMlModelResolver modelResolver)
    {
        _modelResolver = modelResolver;
    }

    public async Task<DataResult<TireDetectionResult>> DetectTireRimCircle(Image image)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var modelResult = await _modelResolver.Resolve<ITireDetectionService>();
        if (modelResult.IsFailure)
            return DataResult<TireDetectionResult>.Failure(
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

        using var imageToDetect = SKImage.FromBitmap(SKBitmap.Decode(image.Data));
        var results = yolo.RunSegmentation(imageToDetect);
        var rimMaskResult = results.FirstOrDefault(res =>
            string.Equals(res.Label.Name, RimClassName, StringComparison.CurrentCultureIgnoreCase));
        if (rimMaskResult is null || rimMaskResult.Confidence < ConfidenceThreshold)
            return DataResult<TireDetectionResult>.NotFound("No tire rim was detected in the image.");

        using var mask = ConvertSegmentationMaskToMat(
                detectedImage: imageToDetect,
                segmentationResult: rimMaskResult)
            .Threshold(1, 255, ThresholdTypes.Binary);

        using var invertedMask = new Mat();
        Cv2.BitwiseNot(mask, invertedMask);

        invertedMask.FindContours(out var contours, out _,
            RetrievalModes.External,
            ContourApproximationModes.ApproxSimple);
        var largestContour = contours.MaxBy(p => p.Length);
        if (largestContour is null)
            return DataResult<TireDetectionResult>.NotFound("No tire rim was detected in the image.");

        Cv2.MinEnclosingCircle(largestContour, out var center, out var radius);
        var rimCircle = new CircleInImage
        {
            Center = new ImageCoordinate((int)center.X, (int)center.Y),
            Radius = radius
        };

        stopWatch.Stop();
        var timeTaken = stopWatch.Elapsed;
        var result = new TireDetectionResult(rimCircle, timeTaken);

        return DataResult<TireDetectionResult>.Success(result);
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
}