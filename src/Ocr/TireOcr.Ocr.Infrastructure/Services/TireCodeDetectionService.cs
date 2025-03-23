using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services;

public class TireCodeDetectionService : ITireCodeDetectionService
{
    private readonly ITireCodeDetectorResolver _tireCodeDetectorResolver;

    public TireCodeDetectionService(ITireCodeDetectorResolver tireCodeDetectorResolver)
    {
        _tireCodeDetectorResolver = tireCodeDetectorResolver;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(TireCodeDetectorType detectorType, Image image)
    {
        var detectorResult = _tireCodeDetectorResolver.Resolve(detectorType);
        return await detectorResult.MapAsync(
            onSuccess: detector => detector.DetectAsync(image),
            onFailure: f => Task.FromResult(DataResult<OcrResultDto>.Failure(f))
        );
    }
}