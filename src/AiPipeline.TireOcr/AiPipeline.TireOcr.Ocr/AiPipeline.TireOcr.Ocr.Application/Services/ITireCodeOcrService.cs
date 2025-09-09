using AiPipeline.TireOcr.Shared.Models;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Application.Services;

public interface ITireCodeOcrService
{
    public Task<DataResult<OcrResultDto>> DetectAsync(TireCodeDetectorType detectorType, Image image);
}