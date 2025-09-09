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

    public TireCodeOcrService(ITireCodeDetectorResolverService tireCodeDetectorResolverService)
    {
        _tireCodeDetectorResolverService = tireCodeDetectorResolverService;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(TireCodeDetectorType detectorType, Image image)
    {
        var detectorResult = _tireCodeDetectorResolverService.Resolve(detectorType);
        return await detectorResult.MapAsync(
            onSuccess: detector => detector.DetectAsync(image),
            onFailure: f => Task.FromResult(DataResult<OcrResultDto>.Failure(f))
        );
    }
}