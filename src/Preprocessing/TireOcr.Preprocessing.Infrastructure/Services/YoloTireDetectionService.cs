using OpenCvSharp;
using SkiaSharp;
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
    private readonly IMlModelResolver _modelResolver;

    public YoloTireDetectionService(IMlModelResolver modelResolver)
    {
        _modelResolver = modelResolver;
    }

    public async Task<DataResult<CircleInImage>> DetectTireRimCircle(Image image)
    {
        var modelToUse = _modelResolver.Resolve<YoloTireDetectionService>();
        if (modelToUse is null)
            return DataResult<CircleInImage>.Failure(new Failure(500, "Failed to load Ml model for tire detection"));

        using var yolo = new Yolo(new YoloOptions
        {
            OnnxModel = modelToUse.GetAbsolutePath(),
            ModelType = ModelType.Segmentation,
            Cuda = false,
            // GpuId = 0,
            PrimeGpu = false
        });

        using var imageToDetect = SKImage.FromBitmap(SKBitmap.Decode(image.Data));
        var results = yolo.RunSegmentation(imageToDetect);
        var rimMaskResult = results.FirstOrDefault(res =>
            string.Equals(res.Label.Name, RimClassName, StringComparison.CurrentCultureIgnoreCase));
        if (rimMaskResult is null)
            return DataResult<CircleInImage>.NotFound("No tire rim was detected in the image.");


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
            return DataResult<CircleInImage>.NotFound("No tire rim was detected in the image.");

        Cv2.MinEnclosingCircle(largestContour, out var center, out var radius);
        var rimCircle = new CircleInImage
        {
            Center = new ImageCoordinate((int)center.X, (int)center.Y),
            Radius = radius
        };

        return DataResult<CircleInImage>.Success(rimCircle);
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