using SkiaSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using YoloDotNet.Enums;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Models;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;
using TireOcr.Shared.Result;
using YoloDotNet;
using YoloDotNet.Extensions;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class YoloTextDetectionService : ITextDetectionService
{
    private const double ConfidenceThreshold = 0.05;
    private readonly IMlModelResolverService _modelResolverService;

    public YoloTextDetectionService(IMlModelResolverService modelResolverService)
    {
        _modelResolverService = modelResolverService;
    }

    public async Task<DataResult<List<CharacterInImage>>> DetectTextInImage(Image image)
    {
        var modelResult = await _modelResolverService.Resolve<ITextDetectionService>();
        if (modelResult.IsFailure)
            return DataResult<List<CharacterInImage>>.Failure(
                new Failure(500, modelResult.PrimaryFailure?.Message ?? "Failed to resolve ML model")
            );

        var model = modelResult.Data!;
        using var yolo = new Yolo(new YoloOptions
        {
            ExecutionProvider = new CpuExecutionProvider(
                model: model.GetAbsolutePath()
            ),
            ImageResize = ImageResize.Proportional,
            SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
        });

        using var imageToDetect = SKImage.FromBitmap(SKBitmap.Decode(image.Data));
        var results = yolo.RunObjectDetection(imageToDetect, confidence: ConfidenceThreshold);

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

        //DrawAndSaveAnnotatedImage(image, imageToDetect, results);

        return DataResult<List<CharacterInImage>>.Success(detectedCharacters);
    }

    private void DrawAndSaveAnnotatedImage(Image originalImage, SKImage image, List<ObjectDetection> results)
    {
        using var annotatedBitmap = image.Draw(results, new DetectionDrawingOptions
        {
            DrawBoundingBoxes = true,
            DrawConfidenceScore = false,
            DrawLabels = true,
            EnableFontShadow = true,
            Font = SKTypeface.Default,
            FontSize = 10,
            FontColor = SKColors.White,
            DrawLabelBackground = true,
            EnableDynamicScaling = true,
            BorderThickness = 2,
            BoundingBoxOpacity = 128,
        });

        var path = Path.Combine(GetAnnotatedModelsStoragePath(), originalImage.Name);
        annotatedBitmap.Save(path, SKEncodedImageFormat.Jpeg, 80);
    }

    private string GetAnnotatedModelsStoragePath()
    {
        var rootPath = new DirectoryInfo(AppContext.BaseDirectory).GetSolutionDirectory();
        return Path.Combine(rootPath, "detection_results");
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