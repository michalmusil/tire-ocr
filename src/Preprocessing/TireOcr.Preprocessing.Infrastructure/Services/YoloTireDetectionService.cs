using System.Data;
using SkiaSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class YoloTireDetectionService : ITireDetectionService
{
    private const string ModelPath = "...";
        

    public async Task<DataResult<CircleInImage>> DetectTireCircle(Image image)
    {
        throw new RowNotInTableException("TODO");
        using var yolo = new Yolo(new YoloOptions
        {
            OnnxModel = @"path\to\model.onnx",
            ModelType = ModelType.ObjectDetection,
            // Cuda = false,
            // GpuId = 0,
            PrimeGpu = false
        });

        var results = yolo.RunSegmentation(SKImage.FromBitmap(SKBitmap.Decode(image.Data)));
    }
}