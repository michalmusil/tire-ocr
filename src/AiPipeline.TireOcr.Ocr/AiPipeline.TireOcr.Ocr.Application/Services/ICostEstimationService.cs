using AiPipeline.TireOcr.Shared.Models;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Application.Services;

public interface ICostEstimationService
{
    public Task<DataResult<EstimatedCostsDto>> GetEstimatedCostsAsync(
        TireCodeDetectorType detectorType,
        OcrRequestBillingDto billing
    );
}