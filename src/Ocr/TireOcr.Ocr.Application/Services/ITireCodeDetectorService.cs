using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Application.Services;

public interface ITireCodeDetectorService
{
    public Task<DataResult<OcrResultDto>> DetectAsync(Image image);
}