using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Application.Services;

public interface ICostEstimationService
{
    public Task<DataResult<EstimatedCostsDto>> GetEstimatedCostsAsync(
        TireCodeDetectorType detectorType,
        OcrRequestBillingDto billing
    );
}