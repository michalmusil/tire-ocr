using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Application.Services;

public interface ITireCodeDetectionService
{
    public Task<DataResult<OcrResultDto>> DetectAsync(TireCodeDetectorType detectorType, Image image);
}