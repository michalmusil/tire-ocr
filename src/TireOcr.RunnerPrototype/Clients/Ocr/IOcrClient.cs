using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Ocr;

public interface IOcrClient
{
    public Task<DataResult<OcrServiceResultDto>> PerformTireCodeOcrOnImage(
        Image image,
        TireCodeDetectorType detectorType
    );
}