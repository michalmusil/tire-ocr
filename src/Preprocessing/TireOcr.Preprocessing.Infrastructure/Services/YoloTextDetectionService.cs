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

public class YoloTextDetectionService : ITextDetectionService
{
    private const double ConfidenceThreshold = 0.05;
    private readonly IMlModelResolver _modelResolver;

    public YoloTextDetectionService(IMlModelResolver modelResolver)
    {
        _modelResolver = modelResolver;
    }

    public async Task<DataResult<List<CharacterInImage>>> DetectTextInImage(Image image)
    {
        var modelToUse = _modelResolver.Resolve<ITextDetectionService>();
        if (modelToUse is null)
            return DataResult<List<CharacterInImage>>.Failure(new Failure(500,
                "Failed to load Ml model for text detection"));

        using var yolo = new Yolo(new YoloOptions
        {
            OnnxModel = modelToUse.GetAbsolutePath(),
            ModelType = ModelType.ObjectDetection,
            Cuda = false,
            PrimeGpu = false
        });

        using var imageToDetect = SKImage.FromBitmap(SKBitmap.Decode(image.Data));
        var results = yolo.RunObjectDetection(imageToDetect);
        if (!results.Any(od => od.Confidence >= ConfidenceThreshold))
            return DataResult<List<CharacterInImage>>.NotFound("Failed to detect text in image");

        var detectedCharacters = results
            .Where(od => od.Confidence >= ConfidenceThreshold)
            .Where(od => !string.IsNullOrEmpty(od.Label.Name))
            .Select(od =>
            {
                var topLeft = new ImageCoordinate(od.BoundingBox.Left, od.BoundingBox.Top);
                var bottomRight = new ImageCoordinate(od.BoundingBox.Right, od.BoundingBox.Bottom);
                var character = GetCharFromLabel(od.Label);
                return new CharacterInImage
                {
                    Character = character,
                    TopLeftCoordinate = topLeft,
                    BottomRightCoordinate = bottomRight
                };
            })
            .ToList();

        return DataResult<List<CharacterInImage>>.Success(detectedCharacters);
    }

    private char GetCharFromLabel(LabelModel label)
    {
        return label.Name switch
        {
            "slash" => '/',
            _ => label.Name[0]
        };
    }
}