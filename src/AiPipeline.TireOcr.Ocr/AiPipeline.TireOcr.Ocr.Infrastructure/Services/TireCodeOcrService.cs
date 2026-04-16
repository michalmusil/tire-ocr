using AiPipeline.TireOcr.Shared.Models;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services;

public class TireCodeOcrService : ITireCodeOcrService
{
    private readonly ITireCodeDetectorResolverService _tireCodeDetectorResolverService;
    private readonly IImageResizeService _imageResizeService;

    public TireCodeOcrService(ITireCodeDetectorResolverService tireCodeDetectorResolverService,
        IImageResizeService imageResizeService)
    {
        _tireCodeDetectorResolverService = tireCodeDetectorResolverService;
        _imageResizeService = imageResizeService;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(TireCodeDetectorType detectorType, Image image,
        ResizeImageToMaxSideOptions? resizeOptions)
    {
        var imageToProcess = image;
        if (resizeOptions is not null)
            imageToProcess = await _imageResizeService.ResizeToRespectMaxSize(imageToProcess, resizeOptions);

        var detectorResult = _tireCodeDetectorResolverService.Resolve(detectorType);
        return await detectorResult.MapAsync(
            onSuccess: detector => detector.DetectAsync(imageToProcess),
            onFailure: f => Task.FromResult(DataResult<OcrResultDto>.Failure(f))
        );
    }
}